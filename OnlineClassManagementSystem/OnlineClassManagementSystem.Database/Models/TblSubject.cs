using System;
using System.Collections.Generic;

namespace OnlineClassManagementSystem.Database.Models;

public partial class TblSubject
{
    public int SubjectId { get; set; }

    public string SubjectName { get; set; } = null!;

    public DateTime CreatedDateTime { get; set; }

    public DateTime ModifiedDateTime { get; set; }

    public bool IsDelete { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual ICollection<TblTeachPlan> TblTeachPlans { get; set; } = new List<TblTeachPlan>();

    public virtual ICollection<TblTimeTable> TblTimeTables { get; set; } = new List<TblTimeTable>();
}
