using System;
using System.Collections.Generic;

namespace OnlineClassManagementSystem.Shared.models.TeachingTracking;

public class TeachingTrackingListRequestModel
{

}

public class TeachingTrackingListResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public List<TeachingTrackingModel> TeachingTrackingList { get; set; } = new();
}

public class TeachingTrackingModel
{
    public int TrackId { get; set; }

    public int TeachPlanId { get; set; }

    public string? TopicTaught { get; set; }

    public int SubClassId { get; set; }

    public string? ClassName { get; set; }

    public int TeacherId { get; set; }

    public string? TeacherName { get; set; }

    public DateOnly DateTaught { get; set; }

    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public string? Remarks { get; set; }

}
