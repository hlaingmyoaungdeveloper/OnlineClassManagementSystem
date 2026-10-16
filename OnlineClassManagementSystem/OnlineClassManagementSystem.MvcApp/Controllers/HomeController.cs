using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineClassManagementSystem.MvcApp.Models;
using OnlineClassManagementSystem.Shared.models.Enrollment;
using OnlineClassManagementSystem.Shared.models.SubClass;
using OnlineClassManagementSystem.Shared.models.TeachingTracking;
using OnlineClassManagementSystem.Shared.models.Timetable;
using OnlineClassManagementSystem.Shared.models.User;
using System.Diagnostics;

namespace OnlineClassManagementSystem.MvcApp.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IHttpClientFactory httpClientFactory, ILogger<HomeController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [ActionName("Index")]
    public async Task<IActionResult> IndexAsync(string? role = "admin")
    {
        var currentRole = (role?.ToLower() == "teacher") ? "teacher" : "admin";
        ViewBag.CurrentRole = currentRole;
        ViewBag.ActiveMenu = "Dashboard";

        var viewModel = new DashboardViewModel { Role = currentRole };

        try
        {
            _logger.LogInformation("Dashboard Index => Fetching dashboard data");
            var client = _httpClientFactory.CreateClient("WebAPI");

            var classesTask = client.GetAsync("api/SubClass");
            var enrollmentsTask = client.GetAsync("api/Enrollment");
            var usersTask = client.GetAsync("api/User");
            var timetablesTask = client.GetAsync("api/Timetables");
            var trackingsTask = client.GetAsync("api/TeachingTracking");

            await Task.WhenAll(classesTask, enrollmentsTask, usersTask, timetablesTask, trackingsTask);

            var classesJson = await (await classesTask).Content.ReadAsStringAsync();
            var enrollmentsJson = await (await enrollmentsTask).Content.ReadAsStringAsync();
            var usersJson = await (await usersTask).Content.ReadAsStringAsync();
            var timetablesJson = await (await timetablesTask).Content.ReadAsStringAsync();
            var trackingsJson = await (await trackingsTask).Content.ReadAsStringAsync();

            var classes = JsonConvert.DeserializeObject<SubClassListResponseModel>(classesJson)?.SubClassList ?? new List<SubClassModel>();
            var enrollments = JsonConvert.DeserializeObject<EnrollmentListResponseModel>(enrollmentsJson)?.EnrollmentList ?? new List<EnrollmentModel>();
            var users = JsonConvert.DeserializeObject<UserListResponseModel>(usersJson)?.Users ?? new List<UserModel>();
            var timetables = JsonConvert.DeserializeObject<TimetableListResponseModel>(timetablesJson)?.TimetableList ?? new List<TimetableModel>();
            var trackings = JsonConvert.DeserializeObject<TeachingTrackingListResponseModel>(trackingsJson)?.TeachingTrackingList ?? new List<TeachingTrackingModel>();

            // Admin metrics
            viewModel.TotalClasses = classes.Count;

            var studentUsers = users.Where(u => string.Equals(u.Role, "Student", StringComparison.OrdinalIgnoreCase)).ToList();
            viewModel.TotalStudents = studentUsers.Count > 0 ? studentUsers.Count : users.Count;

            var teacherUsers = users.Where(u => string.Equals(u.Role, "Teacher", StringComparison.OrdinalIgnoreCase) || string.Equals(u.Role, "Admin", StringComparison.OrdinalIgnoreCase)).ToList();
            viewModel.TotalTeachers = teacherUsers.Count;

            viewModel.TotalEnrollments = enrollments.Count;

            // Monthly enrollments for current year
            var currentYear = DateTime.Now.Year;
            foreach (var en in enrollments)
            {
                if (en.EnrollDate.HasValue && en.EnrollDate.Value.Year == currentYear)
                {
                    int m = en.EnrollDate.Value.Month - 1;
                    if (m >= 0 && m < 12)
                    {
                        viewModel.MonthlyEnrollments[m]++;
                    }
                }
            }

            // Recent Activities
            var activities = new List<RecentActivityItem>();

            foreach (var en in enrollments.OrderByDescending(e => e.EnrollDate ?? DateTime.MinValue).Take(4))
            {
                var dt = en.EnrollDate ?? DateTime.Now;
                activities.Add(new RecentActivityItem
                {
                    Type = "enrollment",
                    Title = $"New enrollment: {en.StudentName ?? ("Student #" + en.StudentId)}",
                    Description = $"Enrolled in {en.ClassName ?? ("Class #" + en.ClassId)}",
                    Timestamp = dt,
                    TimeAgo = FormatTimeAgo(dt)
                });
            }

            foreach (var tr in trackings.OrderByDescending(t => t.DateTaught).Take(3))
            {
                var dt = tr.DateTaught.ToDateTime(tr.StartTime);
                activities.Add(new RecentActivityItem
                {
                    Type = "tracking",
                    Title = $"Test plan taught: {tr.TopicTaught ?? "Session"}",
                    Description = $"{tr.ClassName ?? ("Class #" + tr.SubClassId)} by {tr.TeacherName ?? ("Teacher #" + tr.TeacherId)}",
                    Timestamp = dt,
                    TimeAgo = FormatTimeAgo(dt)
                });
            }

            foreach (var c in classes.OrderByDescending(c => c.OpenDate ?? DateOnly.MinValue).Take(2))
            {
                var dt = c.OpenDate.HasValue ? c.OpenDate.Value.ToDateTime(TimeOnly.MinValue) : DateTime.Now;
                activities.Add(new RecentActivityItem
                {
                    Type = "class",
                    Title = $"Class created: {c.ClassName}",
                    Description = $"Room: {(string.IsNullOrEmpty(c.Place) ? "TBD" : c.Place)} • Capacity: {c.StudentLimit}",
                    Timestamp = dt,
                    TimeAgo = FormatTimeAgo(dt)
                });
            }

            viewModel.RecentActivities = activities.OrderByDescending(a => a.Timestamp).Take(5).ToList();

            // Teacher view metrics
            var currentTeacher = users.FirstOrDefault(u => string.Equals(u.Role, "Teacher", StringComparison.OrdinalIgnoreCase)) ?? users.FirstOrDefault();
            if (currentTeacher != null)
            {
                viewModel.TeacherName = currentTeacher.FullName;
            }

            var teacherSchedules = timetables
                .Where(t => (currentTeacher != null && t.TeacherId == currentTeacher.UserId) ||
                            (currentTeacher != null && string.Equals(t.TeacherName, currentTeacher.FullName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (teacherSchedules.Count == 0 && timetables.Count > 0)
            {
                teacherSchedules = timetables;
            }

            viewModel.UpcomingClasses = teacherSchedules.Take(6).ToList();
            viewModel.MyClassesCount = teacherSchedules.Select(t => t.SubClassId).Distinct().Count();

            var teacherClassIds = teacherSchedules.Select(t => t.SubClassId).Distinct().ToHashSet();
            viewModel.TeacherStudentCount = classes.Where(c => teacherClassIds.Contains(c.SubClassId)).Sum(c => c.StudentCount ?? 0);

            viewModel.PendingTrackingCount = trackings.Count(t =>
                currentTeacher == null || t.TeacherId == currentTeacher.UserId ||
                string.Equals(t.TeacherName, currentTeacher.FullName, StringComparison.OrdinalIgnoreCase));

            _logger.LogInformation("Dashboard Index => Dashboard data loaded successfully");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error fetching dashboard data from API");
            ViewBag.Error = "Cannot connect to Web API. Please verify the API is running.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error loading dashboard data");
            ViewBag.Error = "Error loading dashboard metrics: " + ex.Message;
        }

        return View(viewModel);
    }

    public IActionResult Enrollment(string? role = "admin")
    {
        return RedirectToAction("Index", "Enrollment", new { role });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private static string FormatTimeAgo(DateTime dt)
    {
        var span = DateTime.Now - dt;
        if (span.TotalMinutes < 1) return "Just now";
        if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} min ago";
        if (span.TotalHours < 24) return $"{(int)span.TotalHours} hours ago";
        if (span.TotalDays < 7) return $"{(int)span.TotalDays} days ago";
        return dt.ToString("dd-MM-yyyy");
    }
}
