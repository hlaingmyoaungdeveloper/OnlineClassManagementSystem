using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.Shared.models.SubClass;
using OnlineClassManagementSystem.Shared.models.Enrollment;
using OnlineClassManagementSystem.Shared.models.User;
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

                // Fetch registered students for the enrollment modal
                var users = await _classApiService.GetUsersAsync();
                var studentList = users?.Users?.Where(u => u.Role == "Student").ToList();
                if (studentList == null || studentList.Count == 0)
                {
                    studentList = users?.Users;
                }
                ViewBag.Students = studentList ?? new List<UserModel>();

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

        [HttpGet]
        public async Task<IActionResult> Enroll(int id)
        {
            var response = await _classApiService.GetSubClassAsync(id);
            if (response == null)
            {
                TempData["Error"] = "Class not found.";
                return RedirectToAction(nameof(Index));
            }

            var users = await _classApiService.GetUsersAsync();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(SubClassEnrollRequestModel model)
        {
            var queryRole = Request.Query["role"].ToString();
            var role = !string.IsNullOrEmpty(queryRole) ? queryRole : "admin";

            if (model.ClassId <= 0)
            {
                TempData["Error"] = "Please select a valid class.";
                return RedirectToAction(nameof(Index), new { role });
            }

            try
            {
                if (!model.EnrollDate.HasValue)
                {
                    model.EnrollDate = DateTime.Now;
                }

                int targetStudentId = 0;

                // Flow for NEW student: Request user data and save
                if (model.IsNewStudent)
                {
                    if (string.IsNullOrWhiteSpace(model.FullName) || string.IsNullOrWhiteSpace(model.Email))
                    {
                        TempData["Error"] = "Please provide both Full Name and Email for the new student.";
                        return RedirectToAction(nameof(Index), new { role });
                    }

                    var userCreateModel = new UserCreateRequestModel
                    {
                        FullName = model.FullName.Trim(),
                        Email = model.Email.Trim(),
                        Role = "Student",
                        TelegramUsername = string.IsNullOrWhiteSpace(model.TelegramUsername) ? null : model.TelegramUsername.Trim()
                    };

                    var userResult = await _classApiService.CreateUserAsync(userCreateModel);
                    if (userResult != null && userResult.IsSuccess && userResult.UserId.HasValue && userResult.UserId.Value > 0)
                    {
                        targetStudentId = userResult.UserId.Value;
                    }
                    else
                    {
                        // Check if student already exists or retrieve by email
                        var allUsers = await _classApiService.GetUsersAsync();
                        var found = allUsers?.Users?.FirstOrDefault(u => u.Email.Equals(userCreateModel.Email, StringComparison.OrdinalIgnoreCase));
                        if (found != null && found.UserId > 0)
                        {
                            targetStudentId = found.UserId;
                        }
                        else
                        {
                            TempData["Error"] = userResult?.Message ?? "Failed to register new student.";
                            return RedirectToAction(nameof(Index), new { role });
                        }
                    }

                    if (targetStudentId <= 0)
                    {
                        TempData["Error"] = "Student created, but unable to retrieve Student No for enrollment.";
                        return RedirectToAction(nameof(Index), new { role });
                    }
                }
                else
                {
                    // Existing student selected
                    if (!model.StudentId.HasValue || model.StudentId.Value <= 0)
                    {
                        TempData["Error"] = "Please select a student to enroll.";
                        return RedirectToAction(nameof(Index), new { role });
                    }
                    targetStudentId = model.StudentId.Value;
                }

                // Proceed with enrollment
                var enrollRequest = new EnrollmentCreateRequestModel
                {
                    ClassId = model.ClassId,
                    StudentId = targetStudentId,
                    EnrollDate = model.EnrollDate
                };

                var result = await _classApiService.EnrollStudentAsync(enrollRequest);
                if (result != null && result.IsSuccess)
                {
                    var msg = model.IsNewStudent
                        ? $"New student '{model.FullName}' registered and enrolled successfully!"
                        : (result.Message ?? "Student enrolled successfully!");
                    TempData["Success"] = msg;
                }
                else
                {
                    TempData["Error"] = result?.Message ?? "Failed to enroll student.";
                }
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Cannot connect to Web API. Please check API URL.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred: " + ex.Message;
            }

            return RedirectToAction(nameof(Index), new { role });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("CreateSubClass", new SubClassCreateRequestModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubClassCreateRequestModel model, string? role = "admin")
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all required fields.";
                return RedirectToAction(nameof(Index), new { role });
            }

            try
            {
                var result = await _classApiService.CreateSubClassAsync(model);

                if (result != null && result.IsSuccess)
                {
                    TempData["Success"] = result.Message ?? "Class created successfully!";
                    return RedirectToAction(nameof(Index), new { role });
                }

                TempData["Error"] = result?.Message ?? "Failed to create class. Please check the submitted data.";
                return RedirectToAction(nameof(Index), new { role });
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Cannot connect to Web API. Please check API URL.";
                return RedirectToAction(nameof(Index), new { role });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred: " + ex.Message;
                return RedirectToAction(nameof(Index), new { role });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _classApiService.GetSubClassAsync(id);
            if (response == null)
            {
                TempData["Error"] = "SubClass not found.";
                return RedirectToAction(nameof(Index));
            }
            var requestModel = new SubClassEditRequestModel
            {
                SubClassId = response.SubClassId != 0 ? response.SubClassId : id,
                ClassName = response.ClassName,
                Place = response.Place,
                OpenDate = response.OpenDate,
                OpenTime = response.OpenTime,
                StudentLimit = response.StudentLimit
            };
            return View("EditSubClass", requestModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, SubClassEditRequestModel model)
        {
            if (model.SubClassId == 0 && id.HasValue && id.Value != 0)
            {
                model.SubClassId = id.Value;
            }
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
        [ValidateAntiForgeryToken]
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
                    TempData["Error"] = "Failed to delete class.";
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
