using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineClassManagementSystem.Shared.models.Enrollment;
using OnlineClassManagementSystem.Shared.models.SubClass;
using OnlineClassManagementSystem.Shared.models.User;
using System.Text;

namespace OnlineClassManagementSystem.MvcApp.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<EnrollmentController> _logger;

        public EnrollmentController(IHttpClientFactory httpClientFactory, ILogger<EnrollmentController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // GET: /Enrollment
        [ActionName("Index")]
        public async Task<IActionResult> EnrollmentListAsync(int? classId, string? status, string? search, string? role = "admin", int pageNo = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Enrollment List Async => Fetching enrollments");
                var client = _httpClientFactory.CreateClient("WebAPI");

                ViewBag.CurrentRole = (role?.ToLower() == "teacher") ? "teacher" : "admin";
                ViewBag.ActiveMenu = "Enrollment";
                ViewBag.SelectedClassId = classId;
                ViewBag.SelectedStatus = status ?? "all";
                ViewBag.SearchQuery = search;
                ViewBag.PageNo = pageNo;
                ViewBag.PageSize = pageSize;

                var query = new List<string>
                {
                    $"pageNo={pageNo}",
                    $"pageSize={pageSize}"
                };
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query.Add($"search={Uri.EscapeDataString(search.Trim())}");
                }
                if (classId.HasValue && classId.Value > 0)
                {
                    query.Add($"classId={classId.Value}");
                }
                if (!string.IsNullOrWhiteSpace(status) && !status.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    query.Add($"status={Uri.EscapeDataString(status.Trim())}");
                }

                var httpResponse = await client.GetAsync($"api/Enrollment?{string.Join("&", query)}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<EnrollmentListResponseModel>(jsonString);

                await PopulateDropdowns(client);

                if (response == null)
                {
                    ViewBag.Error = "API returned an empty response.";
                    return View(new EnrollmentListResponseModel { EnrollmentList = new() });
                }

                if (!response.IsSuccess && (response.EnrollmentList == null || response.EnrollmentList.Count == 0))
                {
                    _logger.LogWarning($"Enrollment List Async => Failed to fetch enrollments: {response.Message}");
                    ViewBag.Error = response.Message;
                    TempData["Message"] = response.Message;
                    TempData["isSuccess"] = false;
                }

                var list = response.EnrollmentList ?? new List<EnrollmentModel>();
                ViewData["Enrollments"] = list;
                _logger.LogInformation("Enrollment List Async => Enrollments fetched successfully");

                return View(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EnrollmentListAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        // POST: /Enrollment/UpdateStatus
        [HttpPost]
        [ActionName("UpdateStatus")]
        public async Task<IActionResult> EnrollmentUpdateStatusAsync(int enrollmentId, string status, string? role = "admin")
        {
            try
            {
                _logger.LogInformation($"Enrollment Update Status Async => Updating enrollment {enrollmentId} to {status}");
                var client = _httpClientFactory.CreateClient("WebAPI");

                var request = new EnrollmentUpdateStatusResquestModel
                {
                    EnrollmentId = enrollmentId,
                    Status = status
                };

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var httpResponse = await client.PatchAsync("api/Enrollment", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<EnrollmentUpdateStatusResponseModel>(jsonString)
                    ?? new EnrollmentUpdateStatusResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (result != null && result.IsSuccess)
                {
                    _logger.LogInformation("Enrollment Update Status Async => Status updated successfully");
                    TempData["Message"] = result.Message ?? $"Enrollment status updated to {status}.";
                    TempData["isSuccess"] = true;
                    TempData["Success"] = result.Message ?? $"Enrollment status updated to {status}.";
                }
                else
                {
                    _logger.LogWarning($"Enrollment Update Status Async => Failed to update status: {result?.Message}");
                    TempData["Message"] = result?.Message ?? "Failed to update enrollment status.";
                    TempData["isSuccess"] = false;
                    TempData["Error"] = result?.Message ?? "Failed to update enrollment status.";
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
                {
                    return Json(result);
                }

                return RedirectToAction("Index", new { role });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EnrollmentUpdateStatusAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        // GET: /Enrollment/Create
        [ActionName("Create")]
        public async Task<IActionResult> EnrollmentCreateAsync(string? role = "admin")
        {
            try
            {
                ViewBag.CurrentRole = (role?.ToLower() == "teacher") ? "teacher" : "admin";
                var client = _httpClientFactory.CreateClient("WebAPI");
                await PopulateDropdowns(client);
                return View("Create", new EnrollmentCreateRequestModel { EnrollDate = DateTime.Today });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EnrollmentCreateAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        // POST: /Enrollment/Save
        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> EnrollmentSaveAsync(EnrollmentCreateRequestModel model, string? role = "admin")
        {
            try
            {
                _logger.LogInformation("Enrollment Save Async => Creating enrollment");
                var client = _httpClientFactory.CreateClient("WebAPI");
                ViewBag.CurrentRole = (role?.ToLower() == "teacher") ? "teacher" : "admin";

                if (!ModelState.IsValid)
                {
                    await PopulateDropdowns(client);
                    return View("Create", model);
                }

                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var httpResponse = await client.PostAsync("api/Enrollment", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<EnrollmentCreateResponseModel>(jsonString)
                    ?? new EnrollmentCreateResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (result != null && result.IsSuccess)
                {
                    _logger.LogInformation("Enrollment Save Async => Enrollment created successfully");
                    TempData["Message"] = result.Message ?? "Enrollment created successfully!";
                    TempData["isSuccess"] = true;
                    TempData["Success"] = result.Message ?? "Enrollment created successfully!";
                }
                else
                {
                    _logger.LogWarning($"Enrollment Save Async => Failed to create enrollment: {result?.Message}");
                    TempData["Message"] = result?.Message;
                    TempData["isSuccess"] = false;
                    TempData["Error"] = result?.Message;
                    ViewBag.Error = result?.Message ?? "Failed to create enrollment.";
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
                return View("Create", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EnrollmentSaveAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Create")]
        public Task<IActionResult> EnrollmentCreatePostAsync(EnrollmentCreateRequestModel model, string? role = "admin")
            => EnrollmentSaveAsync(model, role);

        // GET: /Enrollment/Edit/5
        [ActionName("Edit")]
        public async Task<IActionResult> EnrollmentEditAsync(int id, string? role = "admin")
        {
            try
            {
                _logger.LogInformation($"Enrollment Edit Async => Fetching enrollment {id}");
                ViewBag.CurrentRole = (role?.ToLower() == "teacher") ? "teacher" : "admin";
                var client = _httpClientFactory.CreateClient("WebAPI");

                var httpResponse = await client.GetAsync($"api/Enrollment/{id}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var enrollment = JsonConvert.DeserializeObject<EnrollmentEditResponseModel>(jsonString);

                if (enrollment == null || !enrollment.IsSuccess)
                {
                    _logger.LogWarning($"Enrollment Edit Async => Failed to fetch enrollment {id}: {enrollment?.Message}");
                    TempData["Message"] = enrollment?.Message ?? "Enrollment not found.";
                    TempData["isSuccess"] = false;
                    TempData["Error"] = enrollment?.Message ?? "Enrollment not found.";
                    return RedirectToAction("Index", new { role });
                }

                await PopulateDropdowns(client);

                var patchModel = new EnrollmentPatchRequestModel
                {
                    ClassId = enrollment.ClassId,
                    StudentId = enrollment.StudentId,
                    EnrollDate = enrollment.EnrollDate
                };
                ViewBag.EnrollmentId = id;
                ViewBag.StudentName = enrollment.StudentName;
                ViewBag.ClassName = enrollment.ClassName;

                return View("Edit", patchModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EnrollmentEditAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        // POST: /Enrollment/Update/5
        [HttpPost]
        [ActionName("Update")]
        public async Task<IActionResult> EnrollmentUpdateAsync(int id, EnrollmentPatchRequestModel model, string? role = "admin")
        {
            try
            {
                _logger.LogInformation($"Enrollment Update Async => Updating enrollment {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");
                ViewBag.CurrentRole = (role?.ToLower() == "teacher") ? "teacher" : "admin";

                if (!ModelState.IsValid)
                {
                    ViewBag.EnrollmentId = id;
                    await PopulateDropdowns(client);
                    return View("Edit", model);
                }

                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var httpResponse = await client.PatchAsync($"api/Enrollment/{id}", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<EnrollmentPatchResponseModel>(jsonString)
                    ?? new EnrollmentPatchResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (result != null && result.IsSuccess)
                {
                    _logger.LogInformation("Enrollment Update Async => Enrollment updated successfully");
                    TempData["Message"] = result.Message ?? "Enrollment updated successfully!";
                    TempData["isSuccess"] = true;
                    TempData["Success"] = result.Message ?? "Enrollment updated successfully!";
                }
                else
                {
                    _logger.LogWarning($"Enrollment Update Async => Failed to update enrollment: {result?.Message}");
                    TempData["Message"] = result?.Message;
                    TempData["isSuccess"] = false;
                    TempData["Error"] = result?.Message;
                    ViewBag.EnrollmentId = id;
                    ViewBag.Error = result?.Message ?? "Failed to update enrollment.";
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
                {
                    return Json(result);
                }

                if (result != null && result.IsSuccess)
                {
                    return RedirectToAction("Index", new { role });
                }

                ViewBag.EnrollmentId = id;
                await PopulateDropdowns(client);
                return View("Edit", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EnrollmentUpdateAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Edit")]
        public Task<IActionResult> EnrollmentEditPostAsync(int id, EnrollmentPatchRequestModel model, string? role = "admin")
            => EnrollmentUpdateAsync(id, model, role);

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
            var classesResponse = JsonConvert.DeserializeObject<SubClassListResponseModel>(classesJson);
            ViewBag.Classes = classesResponse?.SubClassList ?? new List<SubClassModel>();

            var usersResp = await client.GetAsync("api/User");
            var usersJson = await usersResp.Content.ReadAsStringAsync();
            var usersResponse = JsonConvert.DeserializeObject<UserListResponseModel>(usersJson);
            var students = usersResponse?.Users?.Where(u => u.Role == "Student").ToList();
            if (students == null || students.Count == 0)
            {
                students = usersResponse?.Users ?? new List<UserModel>();
            }
            ViewBag.Students = students;
        }
    }
}
