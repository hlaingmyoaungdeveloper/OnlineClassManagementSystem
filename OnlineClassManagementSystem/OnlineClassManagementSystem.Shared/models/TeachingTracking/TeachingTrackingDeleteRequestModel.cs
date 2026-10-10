using System;

namespace OnlineClassManagementSystem.Shared.models.TeachingTracking;

public class TeachingTrackingDeleteRequestModel
{
    public int TrackId { get; set; }
}

public class TeachingTrackingDeleteResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}
