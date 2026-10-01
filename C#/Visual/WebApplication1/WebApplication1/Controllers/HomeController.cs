using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            List<Student> students = new List<Student>()
            {
                new Student(){ Id=1, Name="Ali", Class="BSCS-5A", CGPA=3.20 },
                new Student(){ Id=2, Name="Ahmed", Class="BSCS-5A", CGPA=3.80 },
                new Student(){ Id=3, Name="Sara", Class="BSCS-5B", CGPA=3.65 },
                new Student(){ Id=4, Name="Fatima", Class="BSCS-5A", CGPA=3.95 },
                new Student(){ Id=5, Name="Usman", Class="BSCS-5B", CGPA=3.45 }
            };

            students = students
                        .OrderByDescending(s => s.CGPA)
                        .ToList();

            return View(students);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Privacy1()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
