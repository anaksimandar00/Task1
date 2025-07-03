using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using Task1.Models;
using Task1.Services;
using System.Drawing.Imaging;
namespace Task1.Controllers
{
    [ApiController]
    public class HomeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        public HomeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("home")]
        public async Task<ContentResult> Index()
        {
            List<EmployeDto> empoyees = await _employeeService.GetEmployess("/gettimeentries");
            string html = GenerateTable(empoyees);

            return new ContentResult()
            {
                Content = html,
                ContentType = "text/html"
            };
        }

        [HttpGet("chart")]
        public async Task<FileContentResult> GetChart()
        {
            var employees = await _employeeService.GetEmployess("/gettimeentries");
            byte[] png = GenerateTotalWorkingHoursChart(employees);

            return File(png, "image/png");
        }

        private byte[] GenerateTotalWorkingHoursChart(List<EmployeDto> employees)
        {
            float totalHours = employees.Sum(e => e.TotalHours);

            using var bitmap = new Bitmap(400, 400);
            using var graphics = Graphics.FromImage(bitmap);

            var rect = new Rectangle(50, 50, 300, 300);
            float startAngle = 0;
            var random = new Random();

            foreach (var item in employees)
            {
                float sweepAngle = item.TotalHours / totalHours * 360f;
                var brush = new SolidBrush(Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)));
                graphics.FillPie(brush, rect, startAngle, sweepAngle);
                startAngle += sweepAngle;
            }

            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);

            return ms.ToArray();
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
            </html>";

            return html;
        }
    }
}
