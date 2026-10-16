using System;

namespace OnlineClassManagementSystem.Shared.models.Timetable;

public class TimetableListResquestModel
{
    public int? PageNo { get; set; }
    public int? PageSize { get; set; }
    public string? Search { get; set; }
}

public class TimetableListResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public List<TimetableModel> TimetableList { get; set; } = new();

    public int TotalCount { get; set; }
    public int PageNo { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    public bool HasPreviousPage => PageNo > 1;
    public bool HasNextPage => PageNo < TotalPages;
}

public class TimetableModel
{
    public int Id { get; set; }

    public int SubClassId { get; set; }

    public string? ClassName { get; set; }

    public int TeacherId { get; set; }

    public string? TeacherName { get; set; }

    public int SubjectId { get; set; }

    public string? SubjectName { get; set; }

    public string DayOfWeek { get; set; } = null!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string Mode { get; set; } = null!;

}

