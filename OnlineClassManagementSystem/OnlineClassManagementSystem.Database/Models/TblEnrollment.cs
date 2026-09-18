using System;
using System.Collections.Generic;

namespace OnlineClassManagementSystem.Database.Models;

public partial class TblEnrollment
{
    public int EnrollmentId { get; set; }

    public int ClassId { get; set; }

    public int StudentId { get; set; }

    public DateTime? EnrollDate { get; set; }

    public virtual TblSubClass Class { get; set; } = null!;

    public virtual TblUser Student { get; set; } = null!;
}
