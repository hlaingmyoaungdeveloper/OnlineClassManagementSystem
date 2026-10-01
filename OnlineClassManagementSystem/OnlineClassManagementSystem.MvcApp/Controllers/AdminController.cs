using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.MvcApp.Services;
using OnlineClassManagementSystem.MvcApp.Services.ApiModels;
using OnlineClassManagementSystem.MvcApp.ViewModels;

namespace OnlineClassManagementSystem.MvcApp.Controllers;

public class AdminController : Controller
{
    private readonly OcmsApiService _api;

    public AdminController(OcmsApiService api)
    {
        _api = api;
    }

    private IActionResult RequireAdmin()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (role != "Admin") return RedirectToAction("Index", "Home");
        return null!;
    }

    // ───────────────────────────── Dashboard ──────────────────────────────
    public async Task<IActionResult> Dashboard()
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;

        var classesTask = _api.GetAllSubClassesAsync();
        var enrollmentsTask = _api.GetAllEnrollmentsAsync();
        await Task.WhenAll(classesTask, enrollmentsTask);

        var classesResult = classesTask.Result;
        var enrollmentsResult = enrollmentsTask.Result;

        var vm = new AdminDashboardViewModel
        {
            TotalClasses = classesResult.SubClassList?.Count ?? 0,
            TotalUsers = 0,  // Will be populated when User API is available
        };

        if (enrollmentsResult.EnrollmentList != null)
        {
            var allEnrollments = enrollmentsResult.EnrollmentList
                .Select(e => new EnrollmentViewModel
                {
                    EnrollmentId = e.EnrollmentId,
                    ClassId = e.ClassId,
                    ClassName = e.ClassName,
                    StudentId = e.StudentId,
                    StudentName = e.StudentName,
                    Status = e.Status,
                    EnrollDate = e.EnrollDate
                }).ToList();

            vm.PendingEnrollments = allEnrollments.Count(e =>
                e.Status?.ToLower() == "pending");
            vm.RecentEnrollments = allEnrollments
                .OrderByDescending(e => e.EnrollDate)
                .Take(10)
                .ToList();
        }

        if (!classesResult.IsSuccess)
            vm.ErrorMessage = classesResult.Message;

        return View(vm);
    }

    // ─────────────────────────── Class Management ─────────────────────────
    public async Task<IActionResult> Classes()
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;

        var result = await _api.GetAllSubClassesAsync();
        var vm = new SubClassListViewModel
        {
            ErrorMessage = result.IsSuccess ? null : result.Message
        };

        if (result.SubClassList != null)
        {
            vm.Classes = result.SubClassList
                .Where(c => !c.IsDelete)
                .Select(c => new SubClassViewModel
                {
                    SubClassId = c.SubClassId,
                    ClassName = c.ClassName,
                    Place = c.Place,
                    OpenDate = c.OpenDate,
                    OpenTime = c.OpenTime,
                    StudentLimit = c.StudentLimit,
                    StudentCount = c.StudentCount ?? 0
                }).ToList();
        }

        return View(vm);
    }

    [HttpGet]
    public IActionResult CreateClass()
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;
        return View(new SubClassFormViewModel { IsEdit = false });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateClass(SubClassFormViewModel form)
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;

        var request = new SubClassCreateApiRequest
        {
            ClassName = form.ClassName,
            Place = form.Place,
            OpenDate = form.OpenDate ?? string.Empty,
            OpenTime = form.OpenTime,
            StudentLimit = form.StudentLimit
        };

        var result = await _api.CreateSubClassAsync(request);
        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Class created successfully!";
            return RedirectToAction(nameof(Classes));
        }

        ViewBag.ErrorMessage = result.Message;
        return View(form);
    }

    [HttpGet]
    public async Task<IActionResult> EditClass(int id)
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;

        var result = await _api.GetSubClassByIdAsync(id);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Classes));
        }

        var vm = new SubClassFormViewModel
        {
            SubClassId = id,
            ClassName = result.ClassName,
            Place = result.Place,
            OpenDate = result.OpenDate,
            OpenTime = result.OpenTime,
            StudentLimit = result.StudentLimit,
            IsEdit = true
        };

        return View("CreateClass", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditClass(int id, SubClassFormViewModel form)
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;

        var request = new SubClassPatchApiRequest
        {
            ClassName = form.ClassName,
            Place = form.Place,
            OpenDate = form.OpenDate,
            OpenTime = form.OpenTime,
            StudentLimit = form.StudentLimit
        };

        var result = await _api.PatchSubClassAsync(id, request);
        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Class updated successfully!";
            return RedirectToAction(nameof(Classes));
        }

        ViewBag.ErrorMessage = result.Message;
        form.IsEdit = true;
        return View("CreateClass", form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteClass(int id)
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;

        var result = await _api.DeleteSubClassAsync(id);
        TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(Classes));
    }

    // ─────────────────────────── Enrollment Requests ──────────────────────
    public async Task<IActionResult> EnrollmentRequests()
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;

        var result = await _api.GetAllEnrollmentsAsync();
        var vm = new EnrollmentListViewModel
        {
            ErrorMessage = result.IsSuccess ? null : result.Message
        };

        if (result.EnrollmentList != null)
        {
            vm.Enrollments = result.EnrollmentList
                .Where(e => !e.IsDelete)
                .Select(e => new EnrollmentViewModel
                {
                    EnrollmentId = e.EnrollmentId,
                    ClassId = e.ClassId,
                    ClassName = e.ClassName,
                    StudentId = e.StudentId,
                    StudentName = e.StudentName,
                    Status = e.Status,
                    EnrollDate = e.EnrollDate
                })
                .OrderByDescending(e => e.EnrollDate)
                .ToList();
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveEnrollment(int id)
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;

        var result = await _api.UpdateEnrollmentStatusAsync(new EnrollmentUpdateStatusApiRequest
        {
            EnrollmentId = id,
            Status = "Approved"
        });

        TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(EnrollmentRequests));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectEnrollment(int id)
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;

        var result = await _api.UpdateEnrollmentStatusAsync(new EnrollmentUpdateStatusApiRequest
        {
            EnrollmentId = id,
            Status = "Rejected"
        });

        TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(EnrollmentRequests));
    }

    // ─────────────────────────── User Management ──────────────────────────
    public IActionResult Users()
    {
        var redirect = RequireAdmin();
        if (redirect != null) return redirect;

        // Placeholder: User list endpoint not yet in WebApi.
        // Will show a coming-soon state gracefully.
        var vm = new UserListViewModel
        {
            ErrorMessage = "User Management API endpoint is not yet available. It will appear here once implemented."
        };
        return View(vm);
    }
}
