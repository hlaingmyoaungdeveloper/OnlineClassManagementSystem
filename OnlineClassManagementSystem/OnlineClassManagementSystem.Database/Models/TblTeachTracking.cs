using System;
using System.Collections.Generic;

namespace OnlineClassManagementSystem.Database.Models;

public partial class TblTeachTracking
{
    public int TrackingId { get; set; }

    public int TeachPlanId { get; set; }

    public int SubClassId { get; set; }

    public int TeacherId { get; set; }

    public DateOnly DateTaught { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string? Remarks { get; set; }

    public bool IsDelete { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public virtual TblTeachPlan TeachPlan { get; set; } = null!;
}
