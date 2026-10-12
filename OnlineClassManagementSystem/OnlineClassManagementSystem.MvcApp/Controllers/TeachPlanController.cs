using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.MvcApp.Services;
using OnlineClassManagementSystem.Shared.models.TeachPlan;

namespace OnlineClassManagementSystem.MvcApp.Controllers
{
    public class TeachPlanController : Controller
    {
        private readonly TeachPlanApiService _teachPlanApiService;

        public TeachPlanController(TeachPlanApiService teachPlanApiService)
        {
            _teachPlanApiService = teachPlanApiService;
        }

        // GET: /TeachPlan
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _teachPlanApiService.GetAllAsync();

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

        // GET: /TeachPlan/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View("CreateTeachPlan", new TeachPlanCreateRequestModel());
        }

        // POST: /TeachPlan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeachPlanCreateRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateTeachPlan", model);
            }

            try
            {
                var result = await _teachPlanApiService.CreateTeachPlanAsync(model);

                if (result != null && result.IsSuccess)
                {
                    TempData["Success"] = result.Message ?? "Teach Plan created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Error = result?.Message ?? "Failed to create Teach Plan. Please check the submitted data.";
                return View("CreateTeachPlan", model);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Cannot connect to Web API. Please check API URL.";
                return View("CreateTeachPlan", model);
            }
        }

        // GET: /TeachPlan/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _teachPlanApiService.GetByIdAsync(id);
                if (response == null || !response.IsSuccess)
                {
                    TempData["Error"] = response?.Message ?? "Teach Plan not found.";
                    return RedirectToAction(nameof(Index));
                }

                var requestModel = new TeachPlanModel
                {
                    Id = response.Id != 0 ? response.Id : id,
                    SubjectId = response.SubjectId,
                    SubjectName = response.SubjectName,
                    Topic = response.Topic
                };

                return View("EditTeachPlan", requestModel);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Cannot connect to Web API. Please check API URL.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /TeachPlan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TeachPlanModel model)
        {
            if (model.Id == 0 && id != 0)
            {
                model.Id = id;
            }

            if (!ModelState.IsValid)
            {
                return View("EditTeachPlan", model);
            }

            try
            {
                var patchModel = new TeachPlanPatchRequestModel
                {
                    SubjectId = model.SubjectId,
                    Topic = model.Topic
                };

                bool success = await _teachPlanApiService.UpdateTeachPlanAsync(model.Id, patchModel);
                if (success)
                {
                    TempData["Success"] = "Teach Plan updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Error = "Failed to update teach plan.";
                return View("EditTeachPlan", model);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Cannot connect to Web API. Please check API URL.";
                return View("EditTeachPlan", model);
            }
        }

        // POST: /TeachPlan/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool success = await _teachPlanApiService.DeleteTeachPlanAsync(id);
                if (success)
                {
                    TempData["Success"] = "Teach Plan deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to delete teach plan.";
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
