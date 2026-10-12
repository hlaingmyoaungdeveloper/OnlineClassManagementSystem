using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.MvcApp.Models;
using System.Diagnostics;

namespace OnlineClassManagementSystem.MvcApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string? role = "admin")
        {
            ViewBag.CurrentRole = (role?.ToLower() == "teacher") ? "teacher" : "admin";
            return View();
        }

        public IActionResult Enrollment(string? role = "admin")
        {
            ViewBag.CurrentRole = (role?.ToLower() == "teacher") ? "teacher" : "admin";
            return View();
        }

        public IActionResult Reports(string? role = "admin")
        {
            ViewBag.CurrentRole = (role?.ToLower() == "teacher") ? "teacher" : "admin";
            return View();
        }

        public IActionResult Privacy()
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
