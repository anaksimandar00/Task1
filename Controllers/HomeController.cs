using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Task1.Models;
using Task1.Services;

namespace Task1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        public HomeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            // You can inject services here if needed
        }

        [HttpGet]
        public async Task<ContentResult> Index()
        {
            List<EmployeDto> empoyees = await _employeeService.GetEmployess("/gettimeentries");
            string html = GenerateTable(empoyees);
            
            return new ContentResult(){
                Content = html,
                ContentType = "text/html"
            };
        }
        private string GenerateTable(List<EmployeDto> employes)
        {
            string style = string.Empty;
            string html = @"<!DOCTYPE html>
                <html><head><title>Employee Table</title></head>
                <body>
                    <h1>Employee Report</h1>
                    <table border='1'>
                        <thead><tr><th>EmployeeName</th><th>Total Hours</th></tr></thead>
                        <tbody>";

            foreach (var employe in employes)
            {
                style = employe.TotalHours < 100 ? "style='background-color:red;'" : "";
                html += $"<tr {style}><td>{employe.EmployeeName}</td><td>{employe.TotalHours}</td></tr>";
            }

            html += @"</tbody>
                    </table>
                </body>
            </html>
            ";

            return html;
        }
    }
}
