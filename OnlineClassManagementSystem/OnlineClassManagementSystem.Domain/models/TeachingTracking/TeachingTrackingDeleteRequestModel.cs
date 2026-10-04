using System;

namespace OnlineClassManagementSystem.Domain.models.TeachingTracking;

public class TeachingTrackingDeleteRequestModel
{
    public int TrackId { get; set; }
}

public class TeachingTrackingDeleteResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}
