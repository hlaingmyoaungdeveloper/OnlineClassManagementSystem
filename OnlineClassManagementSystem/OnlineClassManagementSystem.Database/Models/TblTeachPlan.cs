using System;
using System.Collections.Generic;

namespace OnlineClassManagementSystem.Database.Models;

public partial class TblTeachPlan
{
    public int TeachPlanId { get; set; }

    public int SubjectId { get; set; }

    public string Topic { get; set; } = null!;

    public int CreatedBy { get; set; }

    public bool IsDelete { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public virtual TblSubject Subject { get; set; } = null!;

    public virtual ICollection<TblTeachTracking> TblTeachTrackings { get; set; } = new List<TblTeachTracking>();
}
