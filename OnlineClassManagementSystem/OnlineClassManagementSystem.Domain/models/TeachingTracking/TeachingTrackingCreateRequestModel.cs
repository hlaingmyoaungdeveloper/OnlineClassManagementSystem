using System;

namespace OnlineClassManagementSystem.Domain.models.TeachingTracking;

public class TeachingTrackingCreateRequestModel
{
    public int TeachPlanId { get; set; }

    public int SubClassId { get; set; }

    public int TeacherId { get; set; }

    public DateOnly DateTaught { get; set; }

    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }

    public string? Remarks { get; set; }
}

public class TeachingTrackingCreateResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}
