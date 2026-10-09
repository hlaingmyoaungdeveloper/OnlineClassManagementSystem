using System;

namespace OnlineClassManagementSystem.Domain.models.Timetable;

public class TimetablePatchRequestModel
{
    public int? SubClassId { get; set; }

    public int? TeacherId { get; set; }

    public int? SubjectId { get; set; }

    public string? DayOfWeek { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? Mode { get; set; }
}

public class TimetablePatchResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}
