using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.MvcApp.Services;
using OnlineClassManagementSystem.Shared.models.TeachingTracking;

namespace OnlineClassManagementSystem.MvcApp.Controllers
{
    public class TeachingTrackingController : Controller
    {
        private readonly TeachingTrackingApiService _trackingApiService;

        public TeachingTrackingController(TeachingTrackingApiService trackingApiService)
        {
            _trackingApiService = trackingApiService;
        }

        private async Task LoadDropdownDataAsync()
        {
            var teachPlans = await _trackingApiService.GetTeachPlansAsync();
            var subClasses = await _trackingApiService.GetSubClassesAsync();
            var users = await _trackingApiService.GetUsersAsync();

            ViewBag.TeachPlans = teachPlans?.TeachPlanList ?? new();
            ViewBag.SubClasses = subClasses?.SubClassList ?? new();
            ViewBag.Teachers = users?.Users ?? new();
        }

        // GET: /TeachingTracking
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _trackingApiService.GetAllAsync();

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
                ViewBag.Error = "Cannot connect to Web API. Please check API URL.";
                return View();
            }
        }

        // GET: /TeachingTracking/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdownDataAsync();
            var model = new TeachingTrackingCreateRequestModel
            {
                DateTaught = DateOnly.FromDateTime(DateTime.Today),
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(11, 0)
            };
            return View("CreateTeachingTracking", model);
        }

        // POST: /TeachingTracking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeachingTrackingCreateRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync();
                return View("CreateTeachingTracking", model);
            }

            try
            {
                var result = await _trackingApiService.CreateTeachingTrackingAsync(model);

                if (result != null && result.IsSuccess)
                {
                    TempData["Success"] = result.Message ?? "Teaching tracking created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Error = result?.Message ?? "Failed to create teaching tracking record. Please check the submitted data.";
                await LoadDropdownDataAsync();
                return View("CreateTeachingTracking", model);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Cannot connect to Web API. Please check API URL.";
                await LoadDropdownDataAsync();
                return View("CreateTeachingTracking", model);
            }
        }

        // GET: /TeachingTracking/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _trackingApiService.GetByIdAsync(id);
                if (response == null || !response.IsSuccess)
                {
                    TempData["Error"] = response?.Message ?? "Teaching tracking record not found.";
                    return RedirectToAction(nameof(Index));
                }

                await LoadDropdownDataAsync();

                var editModel = new TeachingTrackingEditResponseModel
                {
                    TrackId = response.TrackId != 0 ? response.TrackId : id,
                    TeachPlanId = response.TeachPlanId,
                    TopicTaught = response.TopicTaught,
                    SubClassId = response.SubClassId,
                    ClassName = response.ClassName,
                    TeacherId = response.TeacherId,
                    TeacherName = response.TeacherName,
                    DateTaught = response.DateTaught,
                    StartTime = response.StartTime,
                    EndTime = response.EndTime,
                    Remarks = response.Remarks
                };

                return View("EditTeachingTracking", editModel);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Cannot connect to Web API. Please check API URL.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /TeachingTracking/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TeachingTrackingEditResponseModel model)
        {
            if (model.TrackId == 0 && id != 0)
            {
                model.TrackId = id;
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync();
                return View("EditTeachingTracking", model);
            }

            try
            {
                var patchModel = new TeachingTrackingPatchRequestModel
                {
                    TeachPlanId = model.TeachPlanId,
                    SubClassId = model.SubClassId,
                    TeacherId = model.TeacherId,
                    DateTaught = model.DateTaught,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    Remarks = model.Remarks
                };

                bool success = await _trackingApiService.UpdateTeachingTrackingAsync(model.TrackId, patchModel);
                if (success)
                {
                    TempData["Success"] = "Teaching tracking updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Error = "Failed to update teaching tracking record.";
                await LoadDropdownDataAsync();
                return View("EditTeachingTracking", model);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Cannot connect to Web API. Please check API URL.";
                await LoadDropdownDataAsync();
                return View("EditTeachingTracking", model);
            }
        }

        // POST: /TeachingTracking/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool success = await _trackingApiService.DeleteTeachingTrackingAsync(id);
                if (success)
                {
                    TempData["Success"] = "Teaching tracking record deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to delete teaching tracking record.";
                }
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Cannot connect to Web API. Please check API URL.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
