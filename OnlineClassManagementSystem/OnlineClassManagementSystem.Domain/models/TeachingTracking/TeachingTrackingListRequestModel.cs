using System;
using System.Collections.Generic;

namespace OnlineClassManagementSystem.Domain.models.TeachingTracking;

public class TeachingTrackingListRequestModel
{
    public int? SubClassId { get; set; }
    public int? TeacherId { get; set; }
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

    public int SubClassId { get; set; }

    public string? ClassName { get; set; }

    public int TeacherId { get; set; }

    public string? TeacherName { get; set; }

    public string TopicTaught { get; set; } = null!;

    public DateTime DateTaught { get; set; }

    public TimeOnly Duration { get; set; }

    public string? Remarks { get; set; }

}
