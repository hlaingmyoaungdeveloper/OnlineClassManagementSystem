using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.MvcApp.Services;
using OnlineClassManagementSystem.Shared.models.Timetable;

namespace OnlineClassManagementSystem.MvcApp.Controllers
{
    public class TimetableController : Controller
    {
        private readonly TimetableApiService _timetableApiService;

        public TimetableController(TimetableApiService timetableApiService)
        {
            _timetableApiService = timetableApiService;
        }

        public async Task<IActionResult> Index(int? classId, int? teacherId, string? role)
        {
            ViewBag.ActiveMenu = "Timetable";
            if (!string.IsNullOrEmpty(role))
            {
                ViewBag.CurrentRole = role.ToLower();
            }

            try
            {
                TimetableListResponseModel? response;

                if (classId.HasValue && classId.Value > 0)
                {
                    var classResult = await _timetableApiService.GetByClassAsync(classId.Value);
                    response = new TimetableListResponseModel
                    {
                        IsSuccess = classResult?.IsSuccess ?? false,
                        Message = classResult?.Message ?? "",
                        TimetableList = classResult?.TimetableList ?? new List<TimetableModel>()
                    };
                    ViewBag.SelectedClassId = classId.Value;
                }
                else if (teacherId.HasValue && teacherId.Value > 0)
                {
                    var teacherResult = await _timetableApiService.GetByTeacherAsync(teacherId.Value);
                    response = new TimetableListResponseModel
                    {
                        IsSuccess = teacherResult?.IsSuccess ?? false,
                        Message = teacherResult?.Message ?? "",
                        TimetableList = teacherResult?.TimetableList ?? new List<TimetableModel>()
                    };
                    ViewBag.SelectedTeacherId = teacherId.Value;
                }
                else
                {
                    response = await _timetableApiService.GetAllAsync();
                }

                if (response == null)
                {
                    ViewBag.Error = "API returned an empty response.";
                    await PopulateDropdowns();
                    return View();
                }

                if (!response.IsSuccess && (response.TimetableList == null || response.TimetableList.Count == 0))
                {
                    ViewBag.Error = response.Message;
                }

                // Populate filter and creation options
                await PopulateDropdowns();

                return View(response);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Cannot connect to Web API. Please check API URL.";
                await PopulateDropdowns();
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(string? day, string? startTime, string? endTime, int? classId, int? teacherId, string? role)
        {
            ViewBag.ActiveMenu = "Timetable";
            if (!string.IsNullOrEmpty(role))
            {
                ViewBag.CurrentRole = role.ToLower();
            }

            await PopulateDropdowns();

            TimeOnly start = TimeOnly.TryParse(startTime, out var s) ? s : new TimeOnly(9, 0);
            TimeOnly end = TimeOnly.TryParse(endTime, out var e) ? e : new TimeOnly(10, 30);
            string dayOfWeek = !string.IsNullOrEmpty(day) ? day : "Monday";

            return View("CreateTimetable", new TimetableCreateRequestModel
            {
                SubClassId = classId ?? 0,
                TeacherId = teacherId ?? 0,
                StartTime = start,
                EndTime = end,
                Mode = "ClassRoom",
                DayOfWeek = dayOfWeek
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TimetableCreateRequestModel model, string? role)
        {
            if (!string.IsNullOrEmpty(role))
            {
                ViewBag.CurrentRole = role.ToLower();
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View("CreateTimetable", model);
            }

            try
            {
                var result = await _timetableApiService.CreateTimetableAsync(model);

                if (result != null && result.IsSuccess)
                {
                    TempData["Success"] = result.Message ?? "Timetable created successfully!";
                    return RedirectToAction(nameof(Index), new { role });
                }

                ViewBag.Error = result?.Message ?? "Failed to create timetable.";
                await PopulateDropdowns();
                return View("CreateTimetable", model);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Cannot connect to Web API. Please check API URL.";
                await PopulateDropdowns();
                return View("CreateTimetable", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string? role)
        {
            ViewBag.ActiveMenu = "Timetable";
            if (!string.IsNullOrEmpty(role))
            {
                ViewBag.CurrentRole = role.ToLower();
            }

            try
            {
                var all = await _timetableApiService.GetAllAsync();
                var item = all?.TimetableList?.FirstOrDefault(x => x.Id == id);

                if (item == null)
                {
                    TempData["Error"] = "Timetable schedule not found.";
                    return RedirectToAction(nameof(Index), new { role });
                }

                var patchModel = new TimetablePatchRequestModel
                {
                    SubClassId = item.SubClassId,
                    TeacherId = item.TeacherId,
                    SubjectId = item.SubjectId,
                    DayOfWeek = item.DayOfWeek,
                    StartTime = item.StartTime,
                    EndTime = item.EndTime,
                    Mode = item.Mode
                };

                ViewBag.TimetableId = id;
                await PopulateDropdowns();
                return View("EditTimetable", patchModel);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Cannot connect to Web API. Please check API URL.";
                return RedirectToAction(nameof(Index), new { role });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TimetablePatchRequestModel model, string? role)
        {
            if (!string.IsNullOrEmpty(role))
            {
                ViewBag.CurrentRole = role.ToLower();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.TimetableId = id;
                await PopulateDropdowns();
                return View("EditTimetable", model);
            }

            try
            {
                var result = await _timetableApiService.UpdateTimetableAsync(id, model);
                if (result != null && result.IsSuccess)
                {
                    TempData["Success"] = result.Message ?? "Timetable updated successfully!";
                    return RedirectToAction(nameof(Index), new { role });
                }

                ViewBag.TimetableId = id;
                ViewBag.Error = result?.Message ?? "Failed to update timetable.";
                await PopulateDropdowns();
                return View("EditTimetable", model);
            }
            catch (HttpRequestException)
            {
                ViewBag.TimetableId = id;
                ViewBag.Error = "Cannot connect to Web API. Please check API URL.";
                await PopulateDropdowns();
                return View("EditTimetable", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? role)
        {
            try
            {
                var result = await _timetableApiService.DeleteTimetableAsync(id);
                if (result != null && result.IsSuccess)
                {
                    TempData["Success"] = result.Message ?? "Timetable deleted successfully!";
                }
                else
                {
                    TempData["Error"] = result?.Message ?? "Failed to delete timetable.";
                }
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Cannot connect to Web API. Please check API URL.";
            }
            return RedirectToAction(nameof(Index), new { role });
        }

        private async Task PopulateDropdowns()
        {
            var classes = await _timetableApiService.GetSubClassesAsync();
            ViewBag.Classes = classes?.SubClassList;

            var users = await _timetableApiService.GetUsersAsync();
            ViewBag.Teachers = users?.Users?.Where(u => u.Role == "Teacher" || u.Role == "Admin").ToList() ?? users?.Users;

            var teachPlans = await _timetableApiService.GetTeachPlansAsync();
            ViewBag.TeachPlans = teachPlans?.TeachPlanList;
        }
    }
}
