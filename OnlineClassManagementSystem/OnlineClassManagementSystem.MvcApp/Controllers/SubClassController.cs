using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.Shared.models.SubClass;
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
                var response = await _classApiService.GetAllAsync(new SubClassListRequestModel());

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

        [HttpGet]
        public IActionResult Create()
        {
            return View("CreateSubClass", new SubClassCreateRequestModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubClass(SubClassCreateRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                bool success = await _classApiService.CreateSubClassAsync(model);

                if (success)
                {
                    TempData["Success"] = "Class created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Error = "Failed to create class. Please check the submitted data.";
                return View(model);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Cannot connect to Web API. Please check API URL.";
                return View(model);
            }
        }

         [HttpGet]
         public async Task<IActionResult> Edit(int id)
         {
            var response = await _classApiService.GetSubClassAsync(id);
            if (response == null)
            {
                ViewBag.Error = "SubClass not found.";
                return RedirectToAction(nameof(Index));
            }
            var requestModel = new SubClassEditRequestModel
            {
                SubClassId = response.SubClassId,
                ClassName = response.ClassName,
                Place = response.Place,
                OpenDate = response.OpenDate,
                OpenTime = response.OpenTime,
                StudentLimit = response.StudentLimit
            };
            return View("EditSubClass", requestModel);
         }

         [HttpPost]
         public async Task<IActionResult> Edit(SubClassEditRequestModel model)
         {
             if (!ModelState.IsValid)
             {
                return View("EditSubClass", model);
             }
             try
             {
                 bool success = await _classApiService.UpdateSubClassAsync(model);
                 if (success)
                 {
                    TempData["Success"] = "Class updated successfully!";
                    return RedirectToAction(nameof(Index));
                 }
                 ViewBag.Error = "Failed to update class.";
                 return View("EditSubClass", model);
             }
             catch (HttpRequestException)
             {
                 ViewBag.Error = "Cannot connect to Web API. Please check API URL.";
                 return View("EditSubClass", model);
             }
         }

         [HttpPost]
         public async Task<IActionResult> Delete(int id)
         {
            try
            {
                bool success = await _classApiService.DeleteSubClassAsync(id);
                if (success)
                {
                    TempData["Success"] = "Class deleted successfully!";
                }
                else
                {
                    ViewBag.Error = "Failed to delete class.";
                }
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Cannot connect to Web API. Please check API URL.";
            }
            return RedirectToAction(nameof(Index));
         }
    }
} 


 
