using Task1.Models;

namespace Task1.Services
{
    public interface IEmployeeService
    {
        Task<List<EmployeDto>> GetEmployess(string uri);
    }
}
