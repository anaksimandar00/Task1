using Microsoft.AspNetCore.Mvc;
using Task1.Services;

namespace Task1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        public HomeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            // You can inject services here if needed
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
