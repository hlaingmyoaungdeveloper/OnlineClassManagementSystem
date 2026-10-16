using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineClassManagementSystem.Shared.models.SubClass;
using OnlineClassManagementSystem.Shared.models.TeachPlan;
using OnlineClassManagementSystem.Shared.models.TeachingTracking;
using OnlineClassManagementSystem.Shared.models.User;
using System.Text;

namespace OnlineClassManagementSystem.MvcApp.Controllers
{
    public class TeachingTrackingController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TeachingTrackingController> _logger;

        public TeachingTrackingController(IHttpClientFactory httpClientFactory, ILogger<TeachingTrackingController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        private async Task LoadDropdownDataAsync(HttpClient client)
        {
            var teachPlansResp = await client.GetAsync("api/TeachPlan");
            var teachPlansJson = await teachPlansResp.Content.ReadAsStringAsync();
            var teachPlans = JsonConvert.DeserializeObject<TeachPlanListResponseModel>(teachPlansJson);

            var subClassesResp = await client.GetAsync("api/SubClass");
            var subClassesJson = await subClassesResp.Content.ReadAsStringAsync();
            var subClasses = JsonConvert.DeserializeObject<SubClassListResponseModel>(subClassesJson);

            var usersResp = await client.GetAsync("api/User");
            var usersJson = await usersResp.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<UserListResponseModel>(usersJson);

            ViewBag.TeachPlans = teachPlans?.TeachPlanList ?? new();
            ViewBag.SubClasses = subClasses?.SubClassList ?? new();
            ViewBag.Teachers = users?.Users ?? new();
        }

        // GET: /TeachingTracking
        [ActionName("Index")]
        public async Task<IActionResult> TeachingTrackingListAsync(string? search = null, string? role = null, int pageNo = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("TeachingTracking List Async => Fetching teaching tracking records");
                var client = _httpClientFactory.CreateClient("WebAPI");

                ViewBag.ActiveMenu = "Tracking";
                ViewBag.SearchQuery = search;
                ViewBag.PageNo = pageNo;
                ViewBag.PageSize = pageSize;
                if (!string.IsNullOrEmpty(role))
                {
                    ViewBag.CurrentRole = role.ToLower();
                }

                var query = new List<string>
                {
                    $"pageNo={pageNo}",
                    $"pageSize={pageSize}"
                };
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query.Add($"search={Uri.EscapeDataString(search.Trim())}");
                }

                var httpResponse = await client.GetAsync($"api/TeachingTracking?{string.Join("&", query)}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<TeachingTrackingListResponseModel>(jsonString);

                if (response == null)
                {
                    ViewBag.Error = "API returned an empty response.";
                    return View(new TeachingTrackingListResponseModel { TeachingTrackingList = new() });
                }

                if (!response.IsSuccess)
                {
                    _logger.LogWarning($"TeachingTracking List Async => Failed to fetch records: {response.Message}");
                    ViewBag.Error = response.Message;
                    TempData["Message"] = response.Message;
                    TempData["isSuccess"] = false;
                    return View(response);
                }

                _logger.LogInformation("TeachingTracking List Async => Records fetched successfully");
                ViewData["TeachingTrackings"] = response.TeachingTrackingList;
                return View(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TeachingTrackingListAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        // GET: /TeachingTracking/Create
        [ActionName("Create")]
        public async Task<IActionResult> TeachingTrackingCreateAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("WebAPI");
                await LoadDropdownDataAsync(client);
                var model = new TeachingTrackingCreateRequestModel
                {
                    DateTaught = DateOnly.FromDateTime(DateTime.Today),
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(11, 0)
                };
                return View("CreateTeachingTracking", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TeachingTrackingCreateAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        // POST: /TeachingTracking/Save
        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> TeachingTrackingSaveAsync(TeachingTrackingCreateRequestModel model)
        {
            try
            {
                _logger.LogInformation("TeachingTracking Save Async => Creating teaching tracking record");
                var client = _httpClientFactory.CreateClient("WebAPI");

                if (!ModelState.IsValid)
                {
                    await LoadDropdownDataAsync(client);
                    return View("CreateTeachingTracking", model);
                }

                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var httpResponse = await client.PostAsync("api/TeachingTracking", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TeachingTrackingCreateResponseModel>(jsonString)
                    ?? new TeachingTrackingCreateResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (result != null && result.IsSuccess)
                {
                    _logger.LogInformation("TeachingTracking Save Async => Record created successfully");
                    TempData["Message"] = result.Message ?? "Teaching tracking created successfully!";
                    TempData["isSuccess"] = true;
                    TempData["Success"] = result.Message ?? "Teaching tracking created successfully!";
                }
                else
                {
                    _logger.LogWarning($"TeachingTracking Save Async => Failed to create record: {result?.Message}");
                    TempData["Message"] = result?.Message;
                    TempData["isSuccess"] = false;
                    TempData["Error"] = result?.Message;
                    ViewBag.Error = result?.Message ?? "Failed to create teaching tracking record. Please check the submitted data.";
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
                {
                    return Json(result);
                }

                if (result != null && result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }

                await LoadDropdownDataAsync(client);
                return View("CreateTeachingTracking", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TeachingTrackingSaveAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Create")]
        public Task<IActionResult> TeachingTrackingCreatePostAsync(TeachingTrackingCreateRequestModel model)
            => TeachingTrackingSaveAsync(model);

        // GET: /TeachingTracking/Edit/5
        [ActionName("Edit")]
        public async Task<IActionResult> TeachingTrackingEditAsync(int id)
        {
            try
            {
                _logger.LogInformation($"TeachingTracking Edit Async => Fetching record {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");

                var httpResponse = await client.GetAsync($"api/TeachingTracking/{id}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<TeachingTrackingEditResponseModel>(jsonString);

                if (response == null || !response.IsSuccess)
                {
                    _logger.LogWarning($"TeachingTracking Edit Async => Failed to fetch record {id}: {response?.Message}");
                    TempData["Message"] = response?.Message ?? "Teaching tracking record not found.";
                    TempData["isSuccess"] = false;
                    TempData["Error"] = response?.Message ?? "Teaching tracking record not found.";
                    return RedirectToAction(nameof(Index));
                }

                await LoadDropdownDataAsync(client);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "TeachingTrackingEditAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        // POST: /TeachingTracking/Update/5
        [HttpPost]
        [ActionName("Update")]
        public async Task<IActionResult> TeachingTrackingUpdateAsync(int id, TeachingTrackingEditResponseModel model)
        {
            try
            {
                _logger.LogInformation($"TeachingTracking Update Async => Updating record {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");

                if (model.TrackId == 0 && id != 0)
                {
                    model.TrackId = id;
                }

                if (!ModelState.IsValid)
                {
                    await LoadDropdownDataAsync(client);
                    return View("EditTeachingTracking", model);
                }

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

                var content = new StringContent(JsonConvert.SerializeObject(patchModel), Encoding.UTF8, "application/json");
                var httpResponse = await client.PatchAsync($"api/TeachingTracking/{model.TrackId}", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TeachingTrackingPatchResponseModel>(jsonString)
                    ?? new TeachingTrackingPatchResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (result != null && result.IsSuccess)
                {
                    _logger.LogInformation("TeachingTracking Update Async => Record updated successfully");
                    TempData["Message"] = result.Message ?? "Teaching tracking updated successfully!";
                    TempData["isSuccess"] = true;
                    TempData["Success"] = result.Message ?? "Teaching tracking updated successfully!";
                }
                else
                {
                    _logger.LogWarning($"TeachingTracking Update Async => Failed to update record: {result?.Message}");
                    TempData["Message"] = result?.Message;
                    TempData["isSuccess"] = false;
                    TempData["Error"] = result?.Message;
                    ViewBag.Error = result?.Message ?? "Failed to update teaching tracking record.";
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
                {
                    return Json(result);
                }

                if (result != null && result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }

                await LoadDropdownDataAsync(client);
                return View("EditTeachingTracking", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TeachingTrackingUpdateAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Edit")]
        public Task<IActionResult> TeachingTrackingEditPostAsync(int id, TeachingTrackingEditResponseModel model)
            => TeachingTrackingUpdateAsync(id, model);

        // POST: /TeachingTracking/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> TeachingTrackingDeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation($"TeachingTracking Delete Async => Deleting record {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");

                var httpResponse = await client.DeleteAsync($"api/TeachingTracking/{id}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TeachingTrackingDeleteResponseModel>(jsonString)
                    ?? new TeachingTrackingDeleteResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (result != null && result.IsSuccess)
                {
                    _logger.LogInformation("TeachingTracking Delete Async => Record deleted successfully");
                    TempData["Message"] = result.Message ?? "Teaching tracking record deleted successfully!";
                    TempData["isSuccess"] = true;
                    TempData["Success"] = result.Message ?? "Teaching tracking record deleted successfully!";
                }
                else
                {
                    _logger.LogWarning($"TeachingTracking Delete Async => Failed to delete record: {result?.Message}");
                    TempData["Message"] = result?.Message;
                    TempData["isSuccess"] = false;
                    TempData["Error"] = result?.Message;
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
                {
                    return Json(result);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TeachingTrackingDeleteAsync => Exception occurred");
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
    }
}
