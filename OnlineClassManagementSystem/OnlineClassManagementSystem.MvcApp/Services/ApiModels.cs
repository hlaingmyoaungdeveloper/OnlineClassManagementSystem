namespace OnlineClassManagementSystem.MvcApp.Services.ApiModels;

// ─── SubClass API Response Models ─────────────────────────────────────────
public class SubClassListApiResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public List<SubClassApiModel>? SubClassList { get; set; }
}

public class SubClassApiModel
{
    public int SubClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string? OpenDate { get; set; }
    public string OpenTime { get; set; } = string.Empty;
    public int StudentLimit { get; set; }
    public int? StudentCount { get; set; }
    public bool IsDelete { get; set; }
}

public class SubClassEditApiResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public int SubClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string? OpenDate { get; set; }
    public string OpenTime { get; set; } = string.Empty;
    public int StudentLimit { get; set; }
    public int? StudentCount { get; set; }
}

public class ApiBaseResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}

// ─── Enrollment API Response Models ───────────────────────────────────────
public class EnrollmentListApiResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public List<EnrollmentApiModel>? EnrollmentList { get; set; }
}

public class EnrollmentApiModel
{
    public int EnrollmentId { get; set; }
    public int ClassId { get; set; }
    public string? ClassName { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? Status { get; set; }
    public DateTime? EnrollDate { get; set; }
    public bool IsDelete { get; set; }
}

// ─── Timetable API Response Models ────────────────────────────────────────
public class TimetableListApiResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public List<TimetableApiModel>? TimetableList { get; set; }
}

public class TimetableApiModel
{
    public int SubClassId { get; set; }
    public string? ClassName { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public int SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
}

// ─── User API Response Models ──────────────────────────────────────────────
public class UserListApiResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public List<UserApiModel>? UserList { get; set; }
}

public class UserApiModel
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedDateTime { get; set; }
    public bool IsDelete { get; set; }
}

// ─── API Request Models ────────────────────────────────────────────────────
public class SubClassCreateApiRequest
{
    public string ClassName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string OpenDate { get; set; } = string.Empty;
    public string OpenTime { get; set; } = string.Empty;
    public int StudentLimit { get; set; }
}

public class SubClassPatchApiRequest
{
    public string? ClassName { get; set; }
    public string? Place { get; set; }
    public string? OpenDate { get; set; }
    public string? OpenTime { get; set; }
    public int? StudentLimit { get; set; }
}

public class EnrollmentCreateApiRequest
{
    public int ClassId { get; set; }
    public int StudentId { get; set; }
    public DateTime? EnrollDate { get; set; }
}

public class EnrollmentUpdateStatusApiRequest
{
    public int EnrollmentId { get; set; }
    public string Status { get; set; } = string.Empty;
}
