using System;

namespace OnlineClassManagementSystem.Domain.models.TeachingTracking;

public class TeachingTrackingPatchRequestModel
{
    public int? SubClassId { get; set; }

    public int? TeacherId { get; set; }

    public string? TopicTaught { get; set; }

    public DateTime? DateTaught { get; set; }

    public TimeOnly? Duration { get; set; }

    public string? Remarks { get; set; }

    public string? Reamrks
    {
        get => Remarks;
        set => Remarks = value;
    }

    public int? ModifiedBy { get; set; }
}

public class TeachingTrackingPatchResponseModel
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}
