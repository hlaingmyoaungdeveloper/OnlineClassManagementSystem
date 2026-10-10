using System;

namespace OnlineClassManagementSystem.Shared.models.TeachingTracking;

public class TeachingTrackingEditRequestModel
{
    public int TrackId { get; set; }
}

public class TeachingTrackingEditResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;

    public int TrackId { get; set; }

    public int TeachPlanId { get; set; }

    public string? TopicTaught { get; set; }

    public int SubClassId { get; set; }

    public string? ClassName { get; set; }

    public int TeacherId { get; set; }

    public string? TeacherName { get; set; }

    public DateOnly DateTaught { get; set; }

    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }

    public string? Remarks { get; set; }
}
