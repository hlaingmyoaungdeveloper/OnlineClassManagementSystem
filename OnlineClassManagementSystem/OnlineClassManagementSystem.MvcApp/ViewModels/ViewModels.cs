namespace OnlineClassManagementSystem.MvcApp.ViewModels;

// ─── SubClass ViewModels ───────────────────────────────────────────────────
public class SubClassViewModel
{
    public int SubClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string? OpenDate { get; set; }
    public string OpenTime { get; set; } = string.Empty;
    public int StudentLimit { get; set; }
    public int StudentCount { get; set; }
    public bool IsDelete { get; set; }

    // Computed helpers
    public double CapacityPercent =>
        StudentLimit > 0 ? Math.Min(100.0, (StudentCount / (double)StudentLimit) * 100) : 0;
    public bool IsFull => StudentCount >= StudentLimit;
    public string CapacityBadge => IsFull ? "Full" : $"{StudentCount}/{StudentLimit}";
}

public class SubClassListViewModel
{
    public List<SubClassViewModel> Classes { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

public class SubClassFormViewModel
{
    public int SubClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string? OpenDate { get; set; }
    public string OpenTime { get; set; } = string.Empty;
    public int StudentLimit { get; set; }
    public bool IsEdit { get; set; }
}

// ─── Enrollment ViewModels ─────────────────────────────────────────────────
public class EnrollmentViewModel
{
    public int EnrollmentId { get; set; }
    public int ClassId { get; set; }
    public string? ClassName { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? Status { get; set; }
    public DateTime? EnrollDate { get; set; }
    public bool IsDelete { get; set; }

    public string StatusBadgeClass => Status?.ToLower() switch
    {
        "approved" => "bg-green-100 text-green-800",
        "rejected" => "bg-red-100 text-red-800",
        _ => "bg-yellow-100 text-yellow-800"
    };
}

public class EnrollmentListViewModel
{
    public List<EnrollmentViewModel> Enrollments { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

// ─── Timetable ViewModels ──────────────────────────────────────────────────
public class TimetableViewModel
{
    public int SubClassId { get; set; }
    public string? ClassName { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public int SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Mode { get; set; } = string.Empty;

    // For calendar grid positioning (8 AM = 0, each hour = 60px)
    public int GridTopPx => (StartTime.Hour - 8) * 60 + StartTime.Minute;
    public int GridHeightPx => (int)((EndTime - StartTime).TotalMinutes);

    public string ModeClass => Mode?.ToLower() switch
    {
        "online" => "bg-blue-500",
        "hybrid" => "bg-purple-500",
        _ => "bg-indigo-600"
    };
}

public class ScheduleViewModel
{
    public List<TimetableViewModel> Entries { get; set; } = new();
    public string[] Days { get; set; } = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
    public string? ErrorMessage { get; set; }

    public IEnumerable<TimetableViewModel> ForDay(string day) =>
        Entries.Where(e => e.DayOfWeek.Equals(day, StringComparison.OrdinalIgnoreCase));
}

// ─── Admin Dashboard ViewModel ─────────────────────────────────────────────
public class AdminDashboardViewModel
{
    public int TotalClasses { get; set; }
    public int TotalUsers { get; set; }
    public int PendingEnrollments { get; set; }
    public List<EnrollmentViewModel> RecentEnrollments { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

// ─── User ViewModels ───────────────────────────────────────────────────────
public class UserViewModel
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedDateTime { get; set; }
}

public class UserListViewModel
{
    public List<UserViewModel> Users { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

// ─── Enroll Request ────────────────────────────────────────────────────────
public class EnrollRequestViewModel
{
    public int ClassId { get; set; }
    public int StudentId { get; set; }
}

// ─── Login ViewModel ───────────────────────────────────────────────────────
public class LoginViewModel
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = "Student";
    public string? ErrorMessage { get; set; }
}
