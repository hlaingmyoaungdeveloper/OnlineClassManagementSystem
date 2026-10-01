using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.MvcApp.Services;
using OnlineClassManagementSystem.MvcApp.Services.ApiModels;
using OnlineClassManagementSystem.MvcApp.ViewModels;

namespace OnlineClassManagementSystem.MvcApp.Controllers;

public class StudentController : Controller
{
    private readonly OcmsApiService _api;

    public StudentController(OcmsApiService api)
    {
        _api = api;
    }

    private (IActionResult? redirect, int studentId) RequireStudent()
    {
        var role = HttpContext.Session.GetString("UserRole");
        var id = HttpContext.Session.GetInt32("UserId") ?? 0;
        if (role != "Student") return (RedirectToAction("Index", "Home"), 0);
        return (null, id);
    }

    // ─────────────────────────── Browse Classes ────────────────────────────
    public async Task<IActionResult> BrowseClasses()
    {
        var (redirect, studentId) = RequireStudent();
        if (redirect != null) return redirect;

        var classesTask = _api.GetAllSubClassesAsync();
        var enrollmentsTask = _api.GetAllEnrollmentsAsync();
        await Task.WhenAll(classesTask, enrollmentsTask);

        var classesResult = classesTask.Result;
        var enrollmentsResult = enrollmentsTask.Result;

        // Find classes the student already enrolled in
        HashSet<int> enrolledClassIds = new();
        if (enrollmentsResult.EnrollmentList != null)
        {
            enrolledClassIds = enrollmentsResult.EnrollmentList
                .Where(e => e.StudentId == studentId && !e.IsDelete)
                .Select(e => e.ClassId)
                .ToHashSet();
        }

        var vm = new SubClassListViewModel
        {
            ErrorMessage = classesResult.IsSuccess ? null : classesResult.Message
        };

        if (classesResult.SubClassList != null)
        {
            vm.Classes = classesResult.SubClassList
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

        ViewBag.StudentId = studentId;
        ViewBag.EnrolledClassIds = enrolledClassIds;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(EnrollRequestViewModel model)
    {
        var (redirect, studentId) = RequireStudent();
        if (redirect != null) return redirect;

        var result = await _api.CreateEnrollmentAsync(new EnrollmentCreateApiRequest
        {
            ClassId = model.ClassId,
            StudentId = studentId,
            EnrollDate = DateTime.Now
        });

        TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(BrowseClasses));
    }

    // ─────────────────────────── My Enrollments ────────────────────────────
    public async Task<IActionResult> MyEnrollments()
    {
        var (redirect, studentId) = RequireStudent();
        if (redirect != null) return redirect;

        var result = await _api.GetAllEnrollmentsAsync();
        var vm = new EnrollmentListViewModel
        {
            ErrorMessage = result.IsSuccess ? null : result.Message
        };

        if (result.EnrollmentList != null)
        {
            vm.Enrollments = result.EnrollmentList
                .Where(e => e.StudentId == studentId && !e.IsDelete)
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

    // ─────────────────────────── My Timetable ─────────────────────────────
    public async Task<IActionResult> MyTimetable()
    {
        var (redirect, studentId) = RequireStudent();
        if (redirect != null) return redirect;

        // Get student's approved enrollments → get timetable for each class
        var enrollResult = await _api.GetAllEnrollmentsAsync();
        var vm = new ScheduleViewModel();

        if (enrollResult.EnrollmentList == null)
        {
            vm.ErrorMessage = enrollResult.Message;
            return View(vm);
        }

        var approvedClassIds = enrollResult.EnrollmentList
            .Where(e => e.StudentId == studentId && !e.IsDelete && e.Status?.ToLower() == "approved")
            .Select(e => e.ClassId)
            .Distinct()
            .ToList();

        var timetableEntries = new List<TimetableViewModel>();
        foreach (var classId in approvedClassIds)
        {
            var ttResult = await _api.GetTimetablesByClassAsync(classId);
            if (ttResult.TimetableList == null) continue;

            timetableEntries.AddRange(ttResult.TimetableList.Select(t => new TimetableViewModel
            {
                SubClassId = t.SubClassId,
                ClassName = t.ClassName,
                TeacherId = t.TeacherId,
                TeacherName = t.TeacherName,
                SubjectId = t.SubjectId,
                SubjectName = t.SubjectName,
                DayOfWeek = t.DayOfWeek,
                StartTime = TimeOnly.TryParse(t.StartTime, out var st) ? st : TimeOnly.MinValue,
                EndTime = TimeOnly.TryParse(t.EndTime, out var et) ? et : TimeOnly.MinValue,
                Mode = t.Mode
            }));
        }

        vm.Entries = timetableEntries;
        return View(vm);
    }
}
