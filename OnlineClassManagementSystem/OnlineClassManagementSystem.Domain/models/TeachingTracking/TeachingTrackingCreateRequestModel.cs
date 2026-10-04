using System;

namespace OnlineClassManagementSystem.Domain.models.TeachingTracking;

public class TeachingTrackingCreateRequestModel
{
    public int SubClassId { get; set; }

    public int TeacherId { get; set; }

    public string TopicTaught { get; set; } = null!;

    public DateTime DateTaught { get; set; }

    public TimeOnly Duration { get; set; }

    public string? Remarks { get; set; }

    public string? Reamrks
    {
        get => Remarks;
        set => Remarks = value;
    }

    public int? CreatedBy { get; set; }
}

public class TeachingTrackingCreateResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}
