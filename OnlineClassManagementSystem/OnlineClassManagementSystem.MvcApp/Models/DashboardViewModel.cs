using OnlineClassManagementSystem.Shared.models.Timetable;

namespace OnlineClassManagementSystem.MvcApp.Models;

public class DashboardViewModel
{
    public string Role { get; set; } = "admin";
    public bool IsTeacher => Role.Equals("teacher", StringComparison.OrdinalIgnoreCase);

    // Admin Metrics
    public int TotalClasses { get; set; }
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalEnrollments { get; set; }
    public int[] MonthlyEnrollments { get; set; } = new int[12];
    public List<RecentActivityItem> RecentActivities { get; set; } = new();

    // Teacher Metrics
    public string TeacherName { get; set; } = "Teacher1";
    public int MyClassesCount { get; set; }
    public int TeacherStudentCount { get; set; }
    public int PendingTrackingCount { get; set; }
    public List<TimetableModel> UpcomingClasses { get; set; } = new();
}

public class RecentActivityItem
{
    public string Type { get; set; } = "info"; // "enrollment", "tracking", "class"
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string TimeAgo { get; set; } = "";
    public DateTime Timestamp { get; set; }
}
