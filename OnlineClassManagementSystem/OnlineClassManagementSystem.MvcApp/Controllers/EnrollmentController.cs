using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.MvcApp.Services;
using OnlineClassManagementSystem.Shared.models.Enrollment;
using OnlineClassManagementSystem.Shared.models.SubClass;
using OnlineClassManagementSystem.Shared.models.User;

namespace OnlineClassManagementSystem.MvcApp.Controllers;

public class EnrollmentController : Controller
{
    private readonly EnrollmentApiService _enrollmentApiService;

    public EnrollmentController(EnrollmentApiService enrollmentApiService)
    {
        _enrollmentApiService = enrollmentApiService;
    }

    // GET: /Enrollment
    public async Task<IActionResult> Index(int? classId, string? status, string? search, string? role = "admin")
    {
        ViewBag.CurrentRole = (role?.ToLower() == "teacher") ? "teacher" : "admin";
        ViewBag.ActiveMenu = "Enrollment";
        ViewBag.SelectedClassId = classId;
        ViewBag.SelectedStatus = status ?? "all";
        ViewBag.SearchQuery = search;

        try
        {
            var response = await _enrollmentApiService.GetAllAsync();
            var classesResponse = await _enrollmentApiService.GetSubClassesAsync();
            var usersResponse = await _enrollmentApiService.GetUsersAsync();

            ViewBag.Classes = classesResponse?.SubClassList ?? new List<SubClassModel>();
            var students = usersResponse?.Users?.Where(u => u.Role == "Student").ToList();
            if (students == null || students.Count == 0)
            {
                students = usersResponse?.Users ?? new List<UserModel>();
            }
            ViewBag.Students = students;

            if (response == null)
            {
                ViewBag.Error = "API returned an empty response.";
                return View(new EnrollmentListResponseModel { EnrollmentList = new() });
            }

            if (!response.IsSuccess && (response.EnrollmentList == null || response.EnrollmentList.Count == 0))
            {
                ViewBag.Error = response.Message;
            }

            var list = response.EnrollmentList ?? new List<EnrollmentModel>();

            // Apply filter by Class
            if (classId.HasValue && classId.Value > 0)
            {
                list = list.Where(e => e.ClassId == classId.Value).ToList();
            }

            // Apply filter by Status
            if (!string.IsNullOrWhiteSpace(status) && !status.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                list = list.Where(e => string.Equals(e.Status, status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Apply search query
            if (!string.IsNullOrWhiteSpace(search))
            {
                var q = search.Trim();
                list = list.Where(e =>
                    (!string.IsNullOrEmpty(e.StudentName) && e.StudentName.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(e.ClassName) && e.ClassName.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                    e.EnrollmentId.ToString().Contains(q)
                ).ToList();
            }

            response.EnrollmentList = list;
            return View(response);
        }
        catch (HttpRequestException)
        {
            ViewBag.Error = "Cannot connect to Web API. Please check API URL.";
            return View(new EnrollmentListResponseModel { EnrollmentList = new() });
        }
    }

    // POST: /Enrollment/UpdateStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int enrollmentId, string status, string? role = "admin")
    {
        try
        {
            var request = new EnrollmentUpdateStatusResquestModel
            {
                EnrollmentId = enrollmentId,
                Status = status
            };

            var result = await _enrollmentApiService.UpdateStatusAsync(request);
            if (result != null && result.IsSuccess)
            {
                TempData["Success"] = result.Message ?? $"Enrollment status updated to {status}.";
            }
            else
            {
                TempData["Error"] = result?.Message ?? "Failed to update enrollment status.";
            }
        }
        catch (HttpRequestException)
        {
            TempData["Error"] = "Cannot connect to Web API. Please check API URL.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { role });
    }

    // GET: /Enrollment/Create
    [HttpGet]
    public async Task<IActionResult> Create(string? role = "admin")
    {
        ViewBag.CurrentRole = (role?.ToLower() == "teacher") ? "teacher" : "admin";
        await PopulateDropdowns();
        return View(new EnrollmentCreateRequestModel { EnrollDate = DateTime.Today });
    }

    // POST: /Enrollment/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EnrollmentCreateRequestModel model, string? role = "admin")
    {
        ViewBag.CurrentRole = (role?.ToLower() == "teacher") ? "teacher" : "admin";

        if (!ModelState.IsValid)
        {
            await PopulateDropdowns();
            return View(model);
        }

        try
        {
            var result = await _enrollmentApiService.CreateEnrollmentAsync(model);
            if (result != null && result.IsSuccess)
            {
                TempData["Success"] = result.Message ?? "Enrollment created successfully!";
                return RedirectToAction(nameof(Index), new { role });
            }

            ViewBag.Error = result?.Message ?? "Failed to create enrollment.";
            await PopulateDropdowns();
            return View(model);
        }
        catch (HttpRequestException)
        {
            ViewBag.Error = "Cannot connect to Web API. Please check API URL.";
            await PopulateDropdowns();
            return View(model);
        }
    }

    // GET: /Enrollment/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id, string? role = "admin")
    {
        ViewBag.CurrentRole = (role?.ToLower() == "teacher") ? "teacher" : "admin";

        try
        {
            var enrollment = await _enrollmentApiService.GetByIdAsync(id);
            if (enrollment == null || !enrollment.IsSuccess)
            {
                TempData["Error"] = enrollment?.Message ?? "Enrollment not found.";
                return RedirectToAction(nameof(Index), new { role });
            }

            await PopulateDropdowns();

            var patchModel = new EnrollmentPatchRequestModel
            {
                ClassId = enrollment.ClassId,
                StudentId = enrollment.StudentId,
                EnrollDate = enrollment.EnrollDate
            };
            ViewBag.EnrollmentId = id;
            ViewBag.StudentName = enrollment.StudentName;
            ViewBag.ClassName = enrollment.ClassName;

            return View(patchModel);
        }
        catch (HttpRequestException)
        {
            TempData["Error"] = "Cannot connect to Web API. Please check API URL.";
            return RedirectToAction(nameof(Index), new { role });
        }
    }

    // POST: /Enrollment/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EnrollmentPatchRequestModel model, string? role = "admin")
    {
        ViewBag.CurrentRole = (role?.ToLower() == "teacher") ? "teacher" : "admin";

        if (!ModelState.IsValid)
        {
            ViewBag.EnrollmentId = id;
            await PopulateDropdowns();
            return View(model);
        }

        try
        {
            var result = await _enrollmentApiService.UpdateEnrollmentAsync(id, model);
            if (result != null && result.IsSuccess)
            {
                TempData["Success"] = result.Message ?? "Enrollment updated successfully!";
                return RedirectToAction(nameof(Index), new { role });
            }

            ViewBag.EnrollmentId = id;
            ViewBag.Error = result?.Message ?? "Failed to update enrollment.";
            await PopulateDropdowns();
            return View(model);
        }
        catch (HttpRequestException)
        {
            ViewBag.EnrollmentId = id;
            ViewBag.Error = "Cannot connect to Web API. Please check API URL.";
            await PopulateDropdowns();
            return View(model);
        }
    }

    private async Task PopulateDropdowns()
    {
        var classesResponse = await _enrollmentApiService.GetSubClassesAsync();
        ViewBag.Classes = classesResponse?.SubClassList ?? new List<SubClassModel>();

        var usersResponse = await _enrollmentApiService.GetUsersAsync();
        var students = usersResponse?.Users?.Where(u => u.Role == "Student").ToList();
        if (students == null || students.Count == 0)
        {
            students = usersResponse?.Users ?? new List<UserModel>();
        }
        ViewBag.Students = students;
    }
}
