using System;
using System.Collections.Generic;

namespace OnlineClassManagementSystem.Database.Models;

public partial class Schedule
{
    public int Id { get; set; }

    public int ClassId { get; set; }

    public int SubjectId { get; set; }

    public int TeacherId { get; set; }

    public string DayOfWeek { get; set; } = null!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string Mode { get; set; } = null!;

    public DateTime CreatedDateTime { get; set; }

    public DateTime ModifiedDateTime { get; set; }

    public bool IsDelete { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual TblSubClass Class { get; set; } = null!;

    public virtual TblSubject Subject { get; set; } = null!;

    public virtual TblUser Teacher { get; set; } = null!;
}
