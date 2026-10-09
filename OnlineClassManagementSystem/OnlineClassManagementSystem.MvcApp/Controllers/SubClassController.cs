using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.MvcApp.Services;

namespace OnlineClassManagementSystem.MvcApp.Controllers
{
    public class SubClassController : Controller
    {
        private readonly SubClassApiService _classApiService;

        public SubClassController(SubClassApiService classApiService)
        {
            _classApiService = classApiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _classApiService.GetAllAsync();

                if (response == null)
                {
                    ViewBag.Error = "API returned an empty response.";
                    return View();
                }

                if (!response.IsSuccess)
                {
                    ViewBag.Error = response.Message;
                    return View();
                }

                return View(response);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error =
                    "Cannot connect to Web API. Please check API URL.";
                return View();
            }
        }
    }
}
 
