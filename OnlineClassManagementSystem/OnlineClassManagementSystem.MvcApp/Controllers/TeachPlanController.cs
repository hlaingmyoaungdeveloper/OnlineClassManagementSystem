using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineClassManagementSystem.Shared.models.TeachPlan;
using System.Text;

namespace OnlineClassManagementSystem.MvcApp.Controllers
{
    public class TeachPlanController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TeachPlanController> _logger;

        public TeachPlanController(IHttpClientFactory httpClientFactory, ILogger<TeachPlanController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // GET: /TeachPlan
        [ActionName("Index")]
        public async Task<IActionResult> TeachPlanListAsync(string? search = null, string? role = null, int pageNo = 1, int pageSize = 10)
        {
            ViewBag.ActiveMenu = "TeachPlan";
            ViewBag.SearchQuery = search ?? "";
            ViewBag.PageNo = pageNo;
            ViewBag.PageSize = pageSize;
            if (!string.IsNullOrEmpty(role))
            {
                ViewBag.CurrentRole = role.ToLower();
            }

            // If requested via AJAX, return JSON data
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
            {
                return await TeachPlanGetDataAsync(search, pageNo, pageSize);
            }

            // Return view instantly without blocking on API
            return View("Index", new TeachPlanListResponseModel 
            { 
                PageNo = pageNo, 
                PageSize = pageSize,
                TeachPlanList = new()
            });
        }

        // GET: /TeachPlan/GetData
        [HttpGet]
        [ActionName("GetData")]
        public async Task<IActionResult> TeachPlanGetDataAsync(string? search = null, int pageNo = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("TeachPlan GetData => Fetching teach plans via AJAX");
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

                var httpResponse = await client.GetAsync($"api/TeachPlan?{string.Join("&", query)}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<TeachPlanListResponseModel>(jsonString)
                    ?? new TeachPlanListResponseModel { IsSuccess = false, Message = "Empty response from API", TeachPlanList = new() };

                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TeachPlanGetDataAsync => Exception occurred");
                return Json(new TeachPlanListResponseModel 
                { 
                    IsSuccess = false, 
                    Message = "Failed to load test plans: " + ex.Message,
                    TeachPlanList = new() 
                });
            }
        }

        // GET: /TeachPlan/Create
        [ActionName("Create")]
        public IActionResult TeachPlanCreate()
        {
            try
            {
                return View("CreateTeachPlan", new TeachPlanCreateRequestModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TeachPlanCreate => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        // POST: /TeachPlan/Save
        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> TeachPlanSaveAsync(TeachPlanCreateRequestModel model)
        {
            try
            {
                _logger.LogInformation("TeachPlan Save Async => Creating teach plan");

                if (!ModelState.IsValid)
                {
                    return View("CreateTeachPlan", model);
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                var httpResponse = await client.PostAsync("api/TeachPlan", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TeachPlanCreateResponseModel>(jsonString)
                    ?? new TeachPlanCreateResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (result != null && result.IsSuccess)
                {
                    _logger.LogInformation("TeachPlan Save Async => Teach plan created successfully");
                    TempData["Message"] = result.Message ?? "Teach Plan created successfully!";
                    TempData["isSuccess"] = true;
                    TempData["Success"] = result.Message ?? "Teach Plan created successfully!";
                }
                else
                {
                    _logger.LogWarning($"TeachPlan Save Async => Failed to create teach plan: {result?.Message}");
                    TempData["Message"] = result?.Message;
                    TempData["isSuccess"] = false;
                    TempData["Error"] = result?.Message;
                    ViewBag.Error = result?.Message ?? "Failed to create Teach Plan. Please check the submitted data.";
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
                {
                    return Json(result);
                }

                if (result != null && result.IsSuccess)
                {
                    return RedirectToAction("Index");
                }

                return View("CreateTeachPlan", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TeachPlanSaveAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Create")]
        public Task<IActionResult> TeachPlanCreatePostAsync(TeachPlanCreateRequestModel model)
            => TeachPlanSaveAsync(model);

        // GET: /TeachPlan/Edit/5
        [ActionName("Edit")]
        public async Task<IActionResult> TeachPlanEditAsync(int id)
        {
            try
            {
                _logger.LogInformation($"TeachPlan Edit Async => Fetching teach plan {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");

                var httpResponse = await client.GetAsync($"api/TeachPlan/{id}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<TeachPlanEditResponseModel>(jsonString);

                if (response == null || !response.IsSuccess)
                {
                    _logger.LogWarning($"TeachPlan Edit Async => Failed to fetch teach plan {id}: {response?.Message}");
                    TempData["Message"] = response?.Message ?? "Teach Plan not found.";
                    TempData["isSuccess"] = false;
                    TempData["Error"] = response?.Message ?? "Teach Plan not found.";
                    return RedirectToAction("Index");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "TeachPlanEditAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        // POST: /TeachPlan/Update/5
        [HttpPost]
        [ActionName("Update")]
        public async Task<IActionResult> TeachPlanUpdateAsync(int id, TeachPlanModel model)
        {
            try
            {
                _logger.LogInformation($"TeachPlan Update Async => Updating teach plan {id}");

                if (model.Id == 0 && id != 0)
                {
                    model.Id = id;
                }

                if (!ModelState.IsValid)
                {
                    return View("EditTeachPlan", model);
                }

                var patchModel = new TeachPlanPatchRequestModel
                {
                    SubjectId = model.SubjectId,
                    Topic = model.Topic
                };

                var client = _httpClientFactory.CreateClient("WebAPI");
                var content = new StringContent(JsonConvert.SerializeObject(patchModel), Encoding.UTF8, "application/json");

                var httpResponse = await client.PatchAsync($"api/TeachPlan/{model.Id}", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TeachPlanPatchResponseModel>(jsonString)
                    ?? new TeachPlanPatchResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (result != null && result.IsSuccess)
                {
                    _logger.LogInformation("TeachPlan Update Async => Teach plan updated successfully");
                    TempData["Message"] = result.Message ?? "Teach Plan updated successfully!";
                    TempData["isSuccess"] = true;
                    TempData["Success"] = result.Message ?? "Teach Plan updated successfully!";
                }
                else
                {
                    _logger.LogWarning($"TeachPlan Update Async => Failed to update teach plan: {result?.Message}");
                    TempData["Message"] = result?.Message;
                    TempData["isSuccess"] = false;
                    TempData["Error"] = result?.Message;
                    ViewBag.Error = result?.Message ?? "Failed to update teach plan.";
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
                {
                    return Json(result);
                }

                if (result != null && result.IsSuccess)
                {
                    return RedirectToAction("Index");
                }

                return View("EditTeachPlan", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TeachPlanUpdateAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Edit")]
        public Task<IActionResult> TeachPlanEditPostAsync(int id, TeachPlanModel model)
            => TeachPlanUpdateAsync(id, model);

        // POST: /TeachPlan/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> TeachPlanDeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation($"TeachPlan Delete Async => Deleting teach plan {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");

                var httpResponse = await client.DeleteAsync($"api/TeachPlan/{id}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TeachPlanDeleteResponseModel>(jsonString)
                    ?? new TeachPlanDeleteResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (result != null && result.IsSuccess)
                {
                    _logger.LogInformation("TeachPlan Delete Async => Teach plan deleted successfully");
                    TempData["Message"] = result.Message ?? "Teach Plan deleted successfully!";
                    TempData["isSuccess"] = true;
                    TempData["Success"] = result.Message ?? "Teach Plan deleted successfully!";
                }
                else
                {
                    _logger.LogWarning($"TeachPlan Delete Async => Failed to delete teach plan: {result?.Message}");
                    TempData["Message"] = result?.Message;
                    TempData["isSuccess"] = false;
                    TempData["Error"] = result?.Message;
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
                {
                    return Json(result);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TeachPlanDeleteAsync => Exception occurred");
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
