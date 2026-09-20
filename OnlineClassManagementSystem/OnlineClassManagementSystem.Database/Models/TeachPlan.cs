using System;
using System.Collections.Generic;

namespace OnlineClassManagementSystem.Database.Models;

public partial class TeachPlan
{
    public int Id { get; set; }

    public int TeacherId { get; set; }

    public int ClassId { get; set; }

    public int SubjectId { get; set; }

    public string Topic { get; set; } = null!;

    public bool? IsCompleted { get; set; }

    public DateTime? CompletedDate { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public DateTime ModifiedDateTime { get; set; }

    public bool IsDelete { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual TblSubClass Class { get; set; } = null!;

    public virtual TblSubject Subject { get; set; } = null!;

    public virtual TblUser Teacher { get; set; } = null!;
}
