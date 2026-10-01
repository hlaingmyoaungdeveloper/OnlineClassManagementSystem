using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.MvcApp.Services;
using OnlineClassManagementSystem.MvcApp.ViewModels;

namespace OnlineClassManagementSystem.MvcApp.Controllers;

public class HomeController : Controller
{
    private readonly OcmsApiService _api;

    public HomeController(OcmsApiService api)
    {
        _api = api;
    }

    [HttpGet]
    public IActionResult Index()
    {
        // If already "logged in" via session, redirect to correct dashboard
        var role = HttpContext.Session.GetString("UserRole");
        return role switch
        {
            "Admin" => RedirectToAction("Dashboard", "Admin"),
            "Teacher" => RedirectToAction("MyClasses", "Teacher"),
            "Student" => RedirectToAction("BrowseClasses", "Student"),
            _ => View()
        };
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (model.UserId <= 0 || string.IsNullOrWhiteSpace(model.Username))
        {
            model.ErrorMessage = "Please provide a valid User ID and Username.";
            return View("Index", model);
        }

        // Store session
        HttpContext.Session.SetInt32("UserId", model.UserId);
        HttpContext.Session.SetString("Username", model.Username);
        HttpContext.Session.SetString("UserRole", model.Role);

        return model.Role switch
        {
            "Admin" => RedirectToAction("Dashboard", "Admin"),
            "Teacher" => RedirectToAction("MyClasses", "Teacher"),
            _ => RedirectToAction("BrowseClasses", "Student")
        };
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
