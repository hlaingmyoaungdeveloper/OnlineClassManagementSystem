using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.MvcApp.Services;
using OnlineClassManagementSystem.MvcApp.ViewModels;

namespace OnlineClassManagementSystem.MvcApp.Controllers;

public class TeacherController : Controller
{
    private readonly OcmsApiService _api;

    public TeacherController(OcmsApiService api)
    {
        _api = api;
    }

    private (IActionResult? redirect, int teacherId) RequireTeacher()
    {
        var role = HttpContext.Session.GetString("UserRole");
        var id = HttpContext.Session.GetInt32("UserId") ?? 0;
        if (role != "Teacher") return (RedirectToAction("Index", "Home"), 0);
        return (null, id);
    }

    // ─────────────────────────── My Classes ───────────────────────────────
    public async Task<IActionResult> MyClasses()
    {
        var (redirect, teacherId) = RequireTeacher();
        if (redirect != null) return redirect;

        // Get timetables for this teacher → derive their classes
        var result = await _api.GetTimetablesByTeacherAsync(teacherId);
        var vm = new SubClassListViewModel
        {
            ErrorMessage = result.IsSuccess ? null : result.Message
        };

        if (result.TimetableList != null)
        {
            // Distinct classes assigned to this teacher
            vm.Classes = result.TimetableList
                .GroupBy(t => t.SubClassId)
                .Select(g => new SubClassViewModel
                {
                    SubClassId = g.Key,
                    ClassName = g.First().ClassName ?? "Unknown",
                    Place = "-",
                    OpenTime = g.First().StartTime
                })
                .ToList();
        }

        return View(vm);
    }

    // ─────────────────────────── My Schedule ──────────────────────────────
    public async Task<IActionResult> MySchedule()
    {
        var (redirect, teacherId) = RequireTeacher();
        if (redirect != null) return redirect;

        var result = await _api.GetTimetablesByTeacherAsync(teacherId);
        var vm = new ScheduleViewModel
        {
            ErrorMessage = result.IsSuccess ? null : result.Message
        };

        if (result.TimetableList != null)
        {
            vm.Entries = result.TimetableList.Select(t => new TimetableViewModel
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
            }).ToList();
        }

        return View(vm);
    }

    // ─────────────────────────── Student Roster ───────────────────────────
    public async Task<IActionResult> StudentRoster(int classId)
    {
        var (redirect, _) = RequireTeacher();
        if (redirect != null) return redirect;

        var enrollResult = await _api.GetAllEnrollmentsAsync();
        var vm = new EnrollmentListViewModel
        {
            ErrorMessage = enrollResult.IsSuccess ? null : enrollResult.Message
        };

        if (enrollResult.EnrollmentList != null)
        {
            vm.Enrollments = enrollResult.EnrollmentList
                .Where(e => !e.IsDelete && e.ClassId == classId && e.Status?.ToLower() == "approved")
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
        }

        ViewBag.ClassId = classId;
        ViewBag.ClassName = vm.Enrollments.FirstOrDefault()?.ClassName ?? $"Class #{classId}";
        return View(vm);
    }
}
