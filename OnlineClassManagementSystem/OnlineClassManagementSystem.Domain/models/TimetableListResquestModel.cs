using System;

namespace OnlineClassManagementSystem.Domain.models;

public class TimetableListResquestModel
{
}

public class TimetableListResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public List<TimetableModel> TimetableList { get; set; }
}

public class TimetableModel
{
    //public int Id { get; set; }

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

    public DateTime CreatedDateTime { get; set; }
}

