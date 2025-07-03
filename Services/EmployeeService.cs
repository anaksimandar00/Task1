
using Task1.Models;

namespace Task1.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public EmployeeService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<EmployeDto>> GetEmployess(string uri)
        {
            List<Employee>? employees = new List<Employee>();
            string? apiUrl = _configuration.GetValue<string>("ApiConfig:BaseUrl");
            string? apiKey = _configuration.GetValue<string>("ApiConfig:ApiKey");

            if (string.IsNullOrEmpty(apiUrl) || string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("API configuration values cannot be null or empty.");
            }

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(apiUrl);

            var response = await client.GetAsync("/api" + uri + "?code=" + apiKey);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                try
                {
                    employees = System.Text.Json.JsonSerializer.Deserialize<List<Employee>>(content);
                }
                catch (System.Text.Json.JsonException ex)
                {
                    throw new InvalidOperationException("Failed to deserialize employee data from API response.", ex);
                }
            }
            else
            {
                throw new HttpRequestException($"Error fetching data from API: {response.ReasonPhrase}");
            }

            if (employees == null)
            {
                throw new InvalidOperationException("Failed to deserialize employee data from API response.");
            }

            // calculating total hours
            List<EmployeDto> result = CalculateWorkingHours(employees);

            return result;
        }

        private List<EmployeDto> CalculateWorkingHours(List<Employee> employees)
        {
            Dictionary<string, EmployeDto> result = new Dictionary<string, EmployeDto>();
            foreach (var employee in employees)
            {
                if (result.ContainsKey(employee.Id))
                {
                    var existingEmployee = result[employee.Id];
                    existingEmployee.TotalHours += (int)(DateTime.Parse(employee.EndTimeUtc) - DateTime.Parse(employee.StarTimeUtc)).TotalHours;
                }
                else
                {
                    result[employee.Id] = new EmployeDto
                    {
                        Id = employee.Id,
                        EmployeeName = employee.EmployeeName,
                        TotalHours = (int)(DateTime.Parse(employee.EndTimeUtc) - DateTime.Parse(employee.StarTimeUtc)).TotalHours
                    };
                }
            }

            return result.Values.ToList();
        }
    }
}
