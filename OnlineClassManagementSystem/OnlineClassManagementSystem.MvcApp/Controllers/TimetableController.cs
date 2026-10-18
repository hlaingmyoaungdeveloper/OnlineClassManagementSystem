using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineClassManagementSystem.Shared.models.SubClass;
using OnlineClassManagementSystem.Shared.models.TeachPlan;
using OnlineClassManagementSystem.Shared.models.Timetable;
using OnlineClassManagementSystem.Shared.models.User;
using System.Text;

namespace OnlineClassManagementSystem.MvcApp.Controllers
{
    public class TimetableController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TimetableController> _logger;

        public TimetableController(IHttpClientFactory httpClientFactory, ILogger<TimetableController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [ActionName("Index")]
        public async Task<IActionResult> TimetableListAsync(int? classId, int? teacherId, string? role)
        {
            ViewBag.ActiveMenu = "Timetable";
            if (!string.IsNullOrEmpty(role))
            {
                ViewBag.CurrentRole = role.ToLower();
            }
            ViewBag.SelectedClassId = classId;
            ViewBag.SelectedTeacherId = teacherId;

            // If requested via AJAX, return JSON data
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
            {
                return await TimetableGetDataAsync(classId, teacherId);
            }

            // Return view instantly without blocking on API
            return View("Index", new TimetableListResponseModel 
            { 
                TimetableList = new() 
            });
        }

        [HttpGet]
        [ActionName("GetData")]
        public async Task<IActionResult> TimetableGetDataAsync(int? classId, int? teacherId)
        {
            try
            {
                _logger.LogInformation("Timetable GetData => Fetching timetables via AJAX");
                var client = _httpClientFactory.CreateClient("WebAPI");

                TimetableListResponseModel? response;

                if (classId.HasValue && classId.Value > 0)
                {
                    var httpResponse = await client.GetAsync($"api/Timetables/club/{classId.Value}");
                    var jsonString = await httpResponse.Content.ReadAsStringAsync();
                    var classResult = JsonConvert.DeserializeObject<TimetableListByClassResponseModel>(jsonString);
                    response = new TimetableListResponseModel
                    {
                        IsSuccess = classResult?.IsSuccess ?? false,
                        Message = classResult?.Message ?? "",
                        TimetableList = classResult?.TimetableList ?? new List<TimetableModel>()
                    };
                }
                else if (teacherId.HasValue && teacherId.Value > 0)
                {
                    var httpResponse = await client.GetAsync($"api/Timetables/teacher/{teacherId.Value}");
                    var jsonString = await httpResponse.Content.ReadAsStringAsync();
                    var teacherResult = JsonConvert.DeserializeObject<TimetableListByTeacherResponseModel>(jsonString);
                    response = new TimetableListResponseModel
                    {
                        IsSuccess = teacherResult?.IsSuccess ?? false,
                        Message = teacherResult?.Message ?? "",
                        TimetableList = teacherResult?.TimetableList ?? new List<TimetableModel>()
                    };
                }
                else
                {
                    var httpResponse = await client.GetAsync("api/Timetables");
                    var jsonString = await httpResponse.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<TimetableListResponseModel>(jsonString);
                }

                return Json(response ?? new TimetableListResponseModel { IsSuccess = false, Message = "Empty response", TimetableList = new() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TimetableGetDataAsync => Exception occurred");
                return Json(new TimetableListResponseModel 
                { 
                    IsSuccess = false, 
                    Message = "Failed to load timetables: " + ex.Message,
                    TimetableList = new() 
                });
            }
        }

        [HttpGet]
        [ActionName("GetDropdownData")]
        public async Task<IActionResult> TimetableGetDropdownDataAsync()
        {
            try
            {
                _logger.LogInformation("Timetable GetDropdownData => Fetching classes, teachers, plans via AJAX");
                var client = _httpClientFactory.CreateClient("WebAPI");

                var classesTask = client.GetAsync("api/SubClass");
                var usersTask = client.GetAsync("api/User");
                var teachPlansTask = client.GetAsync("api/TeachPlan");

                await Task.WhenAll(classesTask, usersTask, teachPlansTask);

                var classesJson = await (await classesTask).Content.ReadAsStringAsync();
                var usersJson = await (await usersTask).Content.ReadAsStringAsync();
                var teachPlansJson = await (await teachPlansTask).Content.ReadAsStringAsync();

                var classes = JsonConvert.DeserializeObject<SubClassListResponseModel>(classesJson);
                var users = JsonConvert.DeserializeObject<UserListResponseModel>(usersJson);
                var teachPlans = JsonConvert.DeserializeObject<TeachPlanListResponseModel>(teachPlansJson);

                return Json(new 
                { 
                    isSuccess = true, 
                    classes = classes?.SubClassList ?? new List<SubClassModel>(),
                    teachers = users?.Users?.Where(u => u.Role == "Teacher" || u.Role == "Admin").ToList() ?? users?.Users ?? new List<UserModel>(),
                    teachPlans = teachPlans?.TeachPlanList ?? new List<TeachPlanModel>()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TimetableGetDropdownDataAsync => Exception occurred");
                return Json(new { isSuccess = false, message = ex.Message });
            }
        }

        [ActionName("Create")]
        public async Task<IActionResult> TimetableCreateAsync(string? day, string? startTime, string? endTime, int? classId, int? teacherId, string? role)
        {
            try
            {
                ViewBag.ActiveMenu = "Timetable";
                if (!string.IsNullOrEmpty(role))
                {
                    ViewBag.CurrentRole = role.ToLower();
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                await PopulateDropdowns(client);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "TimetableCreateAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> TimetableSaveAsync(TimetableCreateRequestModel model, string? role)
        {
            try
            {
                _logger.LogInformation("Timetable Save Async => Creating timetable");
                var client = _httpClientFactory.CreateClient("WebAPI");

                if (!string.IsNullOrEmpty(role))
                {
                    ViewBag.CurrentRole = role.ToLower();
                }

                if (!ModelState.IsValid)
                {
                    await PopulateDropdowns(client);
                    return View("CreateTimetable", model);
                }

                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var httpResponse = await client.PostAsync("api/Timetables", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TimetableCreateResponseModel>(jsonString)
                    ?? new TimetableCreateResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (result != null && result.IsSuccess)
                {
                    _logger.LogInformation("Timetable Save Async => Timetable created successfully");
                    TempData["Message"] = result.Message ?? "Timetable created successfully!";
                    TempData["isSuccess"] = true;
                    TempData["Success"] = result.Message ?? "Timetable created successfully!";
                }
                else
                {
                    _logger.LogWarning($"Timetable Save Async => Failed to create timetable: {result?.Message}");
                    TempData["Message"] = result?.Message;
                    TempData["isSuccess"] = false;
                    TempData["Error"] = result?.Message;
                    ViewBag.Error = result?.Message ?? "Failed to create timetable.";
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
                {
                    return Json(result);
                }

                if (result != null && result.IsSuccess)
                {
                    return RedirectToAction("Index", new { role });
                }

                await PopulateDropdowns(client);
                return View("CreateTimetable", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TimetableSaveAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Create")]
        public Task<IActionResult> TimetableCreatePostAsync(TimetableCreateRequestModel model, string? role)
            => TimetableSaveAsync(model, role);

        [ActionName("Edit")]
        public async Task<IActionResult> TimetableEditAsync(int id, string? role)
        {
            try
            {
                _logger.LogInformation($"Timetable Edit Async => Fetching timetable {id}");
                ViewBag.ActiveMenu = "Timetable";
                if (!string.IsNullOrEmpty(role))
                {
                    ViewBag.CurrentRole = role.ToLower();
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.GetAsync("api/Timetables");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var all = JsonConvert.DeserializeObject<TimetableListResponseModel>(jsonString);
                var item = all?.TimetableList?.FirstOrDefault(x => x.Id == id);

                if (item == null)
                {
                    _logger.LogWarning($"Timetable Edit Async => Timetable schedule {id} not found");
                    TempData["Message"] = "Timetable schedule not found.";
                    TempData["isSuccess"] = false;
                    TempData["Error"] = "Timetable schedule not found.";
                    return RedirectToAction("Index", new { role });
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
                await PopulateDropdowns(client);
                return View("EditTimetable", patchModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TimetableEditAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Update")]
        public async Task<IActionResult> TimetableUpdateAsync(int id, TimetablePatchRequestModel model, string? role)
        {
            try
            {
                _logger.LogInformation($"Timetable Update Async => Updating timetable {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");

                if (!string.IsNullOrEmpty(role))
                {
                    ViewBag.CurrentRole = role.ToLower();
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.TimetableId = id;
                    await PopulateDropdowns(client);
                    return View("EditTimetable", model);
                }

                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var httpResponse = await client.PatchAsync($"api/Timetables/{id}", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TimetablePatchResponseModel>(jsonString)
                    ?? new TimetablePatchResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (result != null && result.IsSuccess)
                {
                    _logger.LogInformation("Timetable Update Async => Timetable updated successfully");
                    TempData["Message"] = result.Message ?? "Timetable updated successfully!";
                    TempData["isSuccess"] = true;
                    TempData["Success"] = result.Message ?? "Timetable updated successfully!";
                }
                else
                {
                    _logger.LogWarning($"Timetable Update Async => Failed to update timetable: {result?.Message}");
                    TempData["Message"] = result?.Message;
                    TempData["isSuccess"] = false;
                    TempData["Error"] = result?.Message;
                    ViewBag.Error = result?.Message ?? "Failed to update timetable.";
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
                {
                    return Json(result);
                }

                if (result != null && result.IsSuccess)
                {
                    return RedirectToAction("Index", new { role });
                }

                ViewBag.TimetableId = id;
                await PopulateDropdowns(client);
                return View("EditTimetable", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TimetableUpdateAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Edit")]
        public Task<IActionResult> TimetableEditPostAsync(int id, TimetablePatchRequestModel model, string? role)
            => TimetableUpdateAsync(id, model, role);

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> TimetableDeleteAsync(int id, string? role)
        {
            try
            {
                _logger.LogInformation($"Timetable Delete Async => Deleting timetable {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");

                var httpResponse = await client.DeleteAsync($"api/Timetables/{id}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TimetableDeleteResponseModel>(jsonString)
                    ?? new TimetableDeleteResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (result != null && result.IsSuccess)
                {
                    _logger.LogInformation("Timetable Delete Async => Timetable deleted successfully");
                    TempData["Message"] = result.Message ?? "Timetable deleted successfully!";
                    TempData["isSuccess"] = true;
                    TempData["Success"] = result.Message ?? "Timetable deleted successfully!";
                }
                else
                {
                    _logger.LogWarning($"Timetable Delete Async => Failed to delete timetable: {result?.Message}");
                    TempData["Message"] = result?.Message;
                    TempData["isSuccess"] = false;
                    TempData["Error"] = result?.Message;
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
                {
                    return Json(result);
                }

                return RedirectToAction("Index", new { role });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TimetableDeleteAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            try
            {
                return View("Error!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        private async Task PopulateDropdowns(HttpClient client)
        {
            var classesResp = await client.GetAsync("api/SubClass");
            var classesJson = await classesResp.Content.ReadAsStringAsync();
            var classes = JsonConvert.DeserializeObject<SubClassListResponseModel>(classesJson);
            ViewBag.Classes = classes?.SubClassList;

            var usersResp = await client.GetAsync("api/User");
            var usersJson = await usersResp.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<UserListResponseModel>(usersJson);
            ViewBag.Teachers = users?.Users?.Where(u => u.Role == "Teacher" || u.Role == "Admin").ToList() ?? users?.Users;

            var teachPlansResp = await client.GetAsync("api/TeachPlan");
            var teachPlansJson = await teachPlansResp.Content.ReadAsStringAsync();
            var teachPlans = JsonConvert.DeserializeObject<TeachPlanListResponseModel>(teachPlansJson);
            ViewBag.TeachPlans = teachPlans?.TeachPlanList;
        }
    }
}
