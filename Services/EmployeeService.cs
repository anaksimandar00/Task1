
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

        public Task GetEmployess()
        {
            string? apiUrl = _configuration.GetValue<string>("ApiConfig:BaseUrl");
            string? apiKey = _configuration.GetValue<string>("ApiConfig:ApiKey");

            if (string.IsNullOrEmpty(apiUrl) || string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("API configuration values cannot be null or empty.");
            }

            // Use apiUrl and apiKey safely here
            // Example: Create an HttpClient and make a request
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(apiUrl + "?code=" + apiKey);
            client.GetAsync("/gettimeentries")
                .ContinueWith(response =>
                {
                    if (response.IsCompletedSuccessfully)
                    {
                        // Handle the response
                        var result = response.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Request successful: " + result.Content);
                        }
                        else
                        {
                            // Handle error
                        }
                    }
                });
            return Task.CompletedTask;
        }
    }
}
