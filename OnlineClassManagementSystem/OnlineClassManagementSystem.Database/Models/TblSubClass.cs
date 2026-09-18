using System;
using System.Collections.Generic;

namespace OnlineClassManagementSystem.Database.Models;

public partial class TblSubClass
{
    public int SubClassId { get; set; }

    public string ClassName { get; set; } = null!;

    public string Place { get; set; } = null!;

    public DateOnly OpenDate { get; set; }

    public string OpenTime { get; set; } = null!;

    public int StudentLimit { get; set; }

    public int? StudentCount { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<TblEnrollment> TblEnrollments { get; set; } = new List<TblEnrollment>();

    public virtual ICollection<TeachPlan> TeachPlans { get; set; } = new List<TeachPlan>();
}
