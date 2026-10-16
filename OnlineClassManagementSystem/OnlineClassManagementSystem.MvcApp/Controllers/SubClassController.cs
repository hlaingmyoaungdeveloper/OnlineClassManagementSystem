using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineClassManagementSystem.Shared.models.Enrollment;
using OnlineClassManagementSystem.Shared.models.SubClass;
using OnlineClassManagementSystem.Shared.models.User;
using System.Text;

namespace OnlineClassManagementSystem.MvcApp.Controllers
{
    public class SubClassController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SubClassController> _logger;

        public SubClassController(IHttpClientFactory httpClientFactory, ILogger<SubClassController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [ActionName("Index")]
        public async Task<IActionResult> SubClassListAsync(int pageNo = 1, int pageSize = 10, string? search = null, string? status = "all")
        {
            try
            {
                _logger.LogInformation("SubClass List Async => Fetching subclasses");

                ViewBag.ActiveMenu = "Class";
                ViewBag.Search = search;
                ViewBag.Status = status ?? "all";
                ViewBag.PageNo = pageNo;
                ViewBag.PageSize = pageSize;

                var client = _httpClientFactory.CreateClient("WebAPI");

                var query = new List<string>
                {
                    $"pageNo={pageNo}",
                    $"pageSize={pageSize}"
                };
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query.Add($"search={Uri.EscapeDataString(search.Trim())}");
                }
                if (!string.IsNullOrWhiteSpace(status) && !status.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    query.Add($"status={Uri.EscapeDataString(status.Trim())}");
                }

                var httpResponse = await client.GetAsync($"api/SubClass?{string.Join("&", query)}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<SubClassListResponseModel>(jsonString);

                // Fetch registered students for the enrollment modal
                var userHttpResponse = await client.GetAsync("api/User");
                var userJsonString = await userHttpResponse.Content.ReadAsStringAsync();
                var userResponse = JsonConvert.DeserializeObject<UserListResponseModel>(userJsonString);
                var studentList = userResponse?.Users?.Where(u => u.Role == "Student").ToList();
                if (studentList == null || studentList.Count == 0)
                {
                    studentList = userResponse?.Users;
                }
                ViewBag.Students = studentList ?? new List<UserModel>();
                ViewData["Students"] = studentList ?? new List<UserModel>();

                if (response != null && response.IsSuccess && response.SubClassList != null)
                {
                    ViewData["SubClasses"] = response.SubClassList;
                    _logger.LogInformation("SubClass List Async => Subclasses fetched successfully");
                }
                else
                {
                    _logger.LogWarning($"SubClass List Async => Failed to fetch subclasses: {response?.Message}");
                    TempData["Message"] = response?.Message;
                    TempData["isSuccess"] = false;
                    TempData["Error"] = response?.Message;
                    ViewBag.Error = response?.Message;
                }

                return View("Index", response ?? new SubClassListResponseModel { SubClassList = new() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SubClassListAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [ActionName("Detail")]
        public async Task<IActionResult> SubClassDetailAsync(int id)
        {
            try
            {
                _logger.LogInformation($"SubClass Detail Async => Fetching subclass {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.GetAsync($"api/SubClass/{id}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<SubClassEditResponseModel>(jsonString);

                if (response == null || !response.IsSuccess)
                {
                    _logger.LogWarning($"SubClass Detail Async => Failed to fetch subclass {id}");
                    TempData["Message"] = "Class not found.";
                    TempData["isSuccess"] = false;
                    TempData["Error"] = "Class not found.";
                    return RedirectToAction("Index");
                }

                ViewData["SubClass"] = response;
                return View("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SubClassDetailAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [ActionName("Create")]
        public IActionResult SubClassCreate()
        {
            try
            {
                return View("CreateSubClass", new SubClassCreateRequestModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SubClassCreate => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> SubClassSaveAsync(SubClassCreateRequestModel requestModel, string? role = "admin")
        {
            try
            {
                _logger.LogInformation("SubClass Save Async => Creating new subclass");

                if (string.IsNullOrWhiteSpace(requestModel.ClassName))
                {
                    _logger.LogWarning("SubClass Save Async => Class Name is required");
                    return Json(new SubClassCreateResponseModel { IsSuccess = false, Message = "Class Name is required." });
                }
                if (string.IsNullOrWhiteSpace(requestModel.Place))
                {
                    _logger.LogWarning("SubClass Save Async => Place is required");
                    return Json(new SubClassCreateResponseModel { IsSuccess = false, Message = "Place is required." });
                }
                if (requestModel.StudentLimit <= 0)
                {
                    _logger.LogWarning("SubClass Save Async => Student limit must be greater than zero");
                    return Json(new SubClassCreateResponseModel { IsSuccess = false, Message = "Student limit must be greater than zero." });
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                var content = new StringContent(JsonConvert.SerializeObject(requestModel), Encoding.UTF8, "application/json");

                var httpResponse = await client.PostAsync("api/SubClass", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<SubClassCreateResponseModel>(jsonString)
                    ?? new SubClassCreateResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (model != null && model.IsSuccess)
                {
                    _logger.LogInformation("SubClass Save Async => Subclass created successfully");
                    TempData["Message"] = model.Message ?? "Class created successfully.";
                    TempData["isSuccess"] = true;
                    TempData["Success"] = model.Message ?? "Class created successfully.";
                }
                else
                {
                    _logger.LogWarning($"SubClass Save Async => Failed to create subclass: {model?.Message}");
                    TempData["Message"] = model?.Message;
                    TempData["isSuccess"] = false;
                    TempData["Error"] = model?.Message;
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
                {
                    return Json(model);
                }

                return RedirectToAction("Index", new { role });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SubClassSaveAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Create")]
        public Task<IActionResult> SubClassCreatePostAsync(SubClassCreateRequestModel requestModel, string? role = "admin")
            => SubClassSaveAsync(requestModel, role);

        [ActionName("Edit")]
        public async Task<IActionResult> SubClassEditAsync(int id)
        {
            try
            {
                _logger.LogInformation($"SubClass Edit Async => Fetching subclass {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.GetAsync($"api/SubClass/{id}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<SubClassEditResponseModel>(jsonString);

                if (model == null || !model.IsSuccess)
                {
                    _logger.LogWarning($"SubClass Edit Async => Failed to fetch subclass {id}: {model?.Message}");
                    TempData["isSuccess"] = false;
                    TempData["Message"] = model?.Message ?? "Class not found.";
                    TempData["Error"] = model?.Message ?? "Class not found.";
                    return RedirectToAction("Index");
                }

                ViewData["Id"] = model.SubClassId;
                ViewData["ClassName"] = model.ClassName;
                ViewData["Place"] = model.Place;
                ViewData["OpenDate"] = model.OpenDate;
                ViewData["OpenTime"] = model.OpenTime;
                ViewData["StudentLimit"] = model.StudentLimit;
                ViewData["StudentCount"] = model.StudentCount;

                var requestModel = new SubClassEditRequestModel
                {
                    SubClassId = model.SubClassId != 0 ? model.SubClassId : id,
                    ClassName = model.ClassName,
                    Place = model.Place,
                    OpenDate = model.OpenDate,
                    OpenTime = model.OpenTime,
                    StudentLimit = model.StudentLimit
                };

                return View("EditSubClass", requestModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SubClassEditAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Update")]
        public async Task<IActionResult> SubClassUpdateAsync(int id, SubClassEditRequestModel requestModel, string? role = "admin")
        {
            try
            {
                var targetId = id != 0 ? id : requestModel.SubClassId;
                _logger.LogInformation($"SubClass Update Async => Updating subclass {targetId}");

                var patchModel = new SubClassPatchRequestModel
                {
                    ClassName = requestModel.ClassName,
                    Place = requestModel.Place,
                    OpenDate = requestModel.OpenDate,
                    OpenTime = requestModel.OpenTime,
                    StudentLimit = requestModel.StudentLimit
                };

                var client = _httpClientFactory.CreateClient("WebAPI");
                var content = new StringContent(JsonConvert.SerializeObject(patchModel), Encoding.UTF8, "application/json");

                var httpResponse = await client.PatchAsync($"api/SubClass/{targetId}", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<SubClassPatchResponseModel>(jsonString)
                    ?? new SubClassPatchResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (model != null && model.IsSuccess)
                {
                    _logger.LogInformation("SubClass Update Async => Subclass updated successfully");
                    TempData["Message"] = model.Message ?? "Class updated successfully.";
                    TempData["isSuccess"] = true;
                    TempData["Success"] = model.Message ?? "Class updated successfully.";
                }
                else
                {
                    _logger.LogWarning($"SubClass Update Async => Failed to update subclass: {model?.Message}");
                    TempData["Message"] = model?.Message;
                    TempData["isSuccess"] = false;
                    TempData["Error"] = model?.Message;
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
                {
                    return Json(model);
                }

                return RedirectToAction("Index", new { role });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SubClassUpdateAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Edit")]
        public Task<IActionResult> SubClassEditPostAsync(int id, SubClassEditRequestModel requestModel, string? role = "admin")
            => SubClassUpdateAsync(id, requestModel, role);

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> SubClassDeleteAsync(int id, SubClassDeleteRequestModel? requestModel, string? role = "admin")
        {
            try
            {
                var targetId = id != 0 ? id : (requestModel?.SubClassId ?? 0);
                _logger.LogInformation($"SubClass Delete Async => Deleting subclass {targetId}");

                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.DeleteAsync($"api/SubClass/{targetId}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<SubClassDeleteResponseModel>(jsonString)
                    ?? new SubClassDeleteResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (model != null && model.IsSuccess)
                {
                    _logger.LogInformation("SubClass Delete Async => Subclass deleted successfully");
                    TempData["Message"] = model.Message ?? "Class deleted successfully.";
                    TempData["isSuccess"] = true;
                    TempData["Success"] = model.Message ?? "Class deleted successfully.";
                }
                else
                {
                    _logger.LogWarning($"SubClass Delete Async => Failed to delete subclass: {model?.Message}");
                    TempData["Message"] = model?.Message;
                    TempData["isSuccess"] = false;
                    TempData["Error"] = model?.Message;
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
                {
                    return Json(model);
                }

                return RedirectToAction("Index", new { role });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SubClassDeleteAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [ActionName("Enroll")]
        public async Task<IActionResult> SubClassEnrollAsync(int id)
        {
            try
            {
                _logger.LogInformation($"SubClass Enroll Async => Fetching subclass {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.GetAsync($"api/SubClass/{id}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<SubClassEditResponseModel>(jsonString);

                if (response == null || !response.IsSuccess)
                {
                    _logger.LogWarning($"SubClass Enroll Async => Failed to fetch subclass {id}");
                    TempData["Message"] = "Class not found.";
                    TempData["isSuccess"] = false;
                    TempData["Error"] = "Class not found.";
                    return RedirectToAction("Index");
                }

                var userResponse = await client.GetAsync("api/User");
                var userJsonString = await userResponse.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<UserListResponseModel>(userJsonString);
                var studentList = users?.Users?.Where(u => u.Role == "Student").ToList();
                if (studentList == null || studentList.Count == 0)
                {
                    studentList = users?.Users;
                }
                ViewBag.Students = studentList ?? new List<UserModel>();
                ViewBag.SubClass = response;

                var model = new SubClassEnrollRequestModel
                {
                    ClassId = response.SubClassId != 0 ? response.SubClassId : id,
                    EnrollDate = DateTime.Today,
                    IsNewStudent = false
                };

                return View("EnrollSubClass", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SubClassEnrollAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Enroll")]
        public async Task<IActionResult> SubClassEnrollSaveAsync(SubClassEnrollRequestModel model)
        {
            try
            {
                var queryRole = Request.Query["role"].ToString();
                var role = !string.IsNullOrEmpty(queryRole) ? queryRole : "admin";

                _logger.LogInformation($"SubClass Enroll Save Async => Enrolling student in class {model.ClassId}");

                if (model.ClassId <= 0)
                {
                    _logger.LogWarning("SubClass Enroll Save Async => Invalid class ID");
                    TempData["Message"] = "Please select a valid class.";
                    TempData["isSuccess"] = false;
                    TempData["Error"] = "Please select a valid class.";
                    return RedirectToAction("Index", new { role });
                }

                if (!model.EnrollDate.HasValue)
                {
                    model.EnrollDate = DateTime.Now;
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                int targetStudentId = 0;

                if (model.IsNewStudent)
                {
                    if (string.IsNullOrWhiteSpace(model.FullName) || string.IsNullOrWhiteSpace(model.Email))
                    {
                        TempData["Message"] = "Please provide both Full Name and Email for the new student.";
                        TempData["isSuccess"] = false;
                        TempData["Error"] = "Please provide both Full Name and Email for the new student.";
                        return RedirectToAction("Index", new { role });
                    }

                    var userCreateModel = new UserCreateRequestModel
                    {
                        FullName = model.FullName.Trim(),
                        Email = model.Email.Trim(),
                        Role = "Student",
                        TelegramUsername = string.IsNullOrWhiteSpace(model.TelegramUsername) ? null : model.TelegramUsername.Trim()
                    };

                    var userContent = new StringContent(JsonConvert.SerializeObject(userCreateModel), Encoding.UTF8, "application/json");
                    var userHttpResponse = await client.PostAsync("api/User", userContent);
                    var userJsonString = await userHttpResponse.Content.ReadAsStringAsync();
                    var userResult = JsonConvert.DeserializeObject<UserCreateResponseModel>(userJsonString);

                    if (userResult != null && userResult.IsSuccess && userResult.UserId.HasValue && userResult.UserId.Value > 0)
                    {
                        targetStudentId = userResult.UserId.Value;
                    }
                    else
                    {
                        var allUsersResp = await client.GetAsync("api/User");
                        var allUsersJson = await allUsersResp.Content.ReadAsStringAsync();
                        var allUsers = JsonConvert.DeserializeObject<UserListResponseModel>(allUsersJson);
                        var found = allUsers?.Users?.FirstOrDefault(u => u.Email.Equals(userCreateModel.Email, StringComparison.OrdinalIgnoreCase));
                        if (found != null && found.UserId > 0)
                        {
                            targetStudentId = found.UserId;
                        }
                        else
                        {
                            TempData["Message"] = userResult?.Message ?? "Failed to register new student.";
                            TempData["isSuccess"] = false;
                            TempData["Error"] = userResult?.Message ?? "Failed to register new student.";
                            return RedirectToAction("Index", new { role });
                        }
                    }

                    if (targetStudentId <= 0)
                    {
                        TempData["Message"] = "Student created, but unable to retrieve Student ID for enrollment.";
                        TempData["isSuccess"] = false;
                        TempData["Error"] = "Student created, but unable to retrieve Student ID for enrollment.";
                        return RedirectToAction("Index", new { role });
                    }
                }
                else
                {
                    if (!model.StudentId.HasValue || model.StudentId.Value <= 0)
                    {
                        TempData["Message"] = "Please select a student to enroll.";
                        TempData["isSuccess"] = false;
                        TempData["Error"] = "Please select a student to enroll.";
                        return RedirectToAction("Index", new { role });
                    }
                    targetStudentId = model.StudentId.Value;
                }

                var enrollRequest = new EnrollmentCreateRequestModel
                {
                    ClassId = model.ClassId,
                    StudentId = targetStudentId,
                    EnrollDate = model.EnrollDate
                };

                var enrollContent = new StringContent(JsonConvert.SerializeObject(enrollRequest), Encoding.UTF8, "application/json");
                var enrollHttpResponse = await client.PostAsync("api/Enrollment", enrollContent);
                var enrollJsonString = await enrollHttpResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<EnrollmentCreateResponseModel>(enrollJsonString);

                if (result != null && result.IsSuccess)
                {
                    var msg = model.IsNewStudent
                        ? $"New student '{model.FullName}' registered and enrolled successfully!"
                        : (result.Message ?? "Student enrolled successfully!");
                    _logger.LogInformation("SubClass Enroll Save Async => Student enrolled successfully");
                    TempData["Message"] = msg;
                    TempData["isSuccess"] = true;
                    TempData["Success"] = msg;
                }
                else
                {
                    _logger.LogWarning($"SubClass Enroll Save Async => Failed to enroll: {result?.Message}");
                    TempData["Message"] = result?.Message ?? "Failed to enroll student.";
                    TempData["isSuccess"] = false;
                    TempData["Error"] = result?.Message ?? "Failed to enroll student.";
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
                {
                    return Json(result);
                }

                return RedirectToAction("Index", new { role });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SubClassEnrollSaveAsync => Exception occurred");
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
