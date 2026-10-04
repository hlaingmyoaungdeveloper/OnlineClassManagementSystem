using System;

namespace OnlineClassManagementSystem.Domain.models.TeachingTracking;

public class TeachingTrackingEditRequestModel
{
    public int TrackId { get; set; }
}

public class TeachingTrackingEditResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;

    public int TrackId { get; set; }

    public int SubClassId { get; set; }

    public string? ClassName { get; set; }

    public int TeacherId { get; set; }

    public string? TeacherName { get; set; }

    public string TopicTaught { get; set; } = null!;

    public DateTime DateTaught { get; set; }

    public TimeOnly Duration { get; set; }

    public string? Remarks { get; set; }

    public string? Reamrks
    {
        get => Remarks;
        set => Remarks = value;
    }

    public DateTime CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
}
