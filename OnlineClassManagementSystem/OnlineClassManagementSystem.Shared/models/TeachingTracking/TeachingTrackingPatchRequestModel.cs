using System;

namespace OnlineClassManagementSystem.Shared.models.TeachingTracking;

public class TeachingTrackingPatchRequestModel
{
    public int? TeachPlanId { get; set; }

    public int? SubClassId { get; set; }

    public int? TeacherId { get; set; }

    public DateOnly? DateTaught { get; set; }

    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }

    public string? Remarks { get; set; }
}

public class TeachingTrackingPatchResponseModel
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}
