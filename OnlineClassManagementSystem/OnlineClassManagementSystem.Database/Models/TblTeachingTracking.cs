using System;
using System.Collections.Generic;

namespace OnlineClassManagementSystem.Database.Models;

public partial class TblTeachingTracking
{
    public int TrackId { get; set; }

    public int SubClassId { get; set; }

    public int TeacherId { get; set; }

    public string TopicTaught { get; set; } = null!;

    public DateTime DateTaught { get; set; }

    public TimeOnly Duration { get; set; }

    public string? Reamrks { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual TblSubClass SubClass { get; set; } = null!;

    public virtual TblUser Teacher { get; set; } = null!;
}
