using System;
using System.Collections.Generic;

namespace OnlineClassManagementSystem.Database.Models;

public partial class TblEnrollment
{
    public int EnrollmentId { get; set; }

    public int ClassId { get; set; }

    public int StudentId { get; set; }

    public DateTime? EnrollDate { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public DateTime ModifiedDateTime { get; set; }

    public bool IsDelete { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual TblSubClass Class { get; set; } = null!;

    public virtual TblUser Student { get; set; } = null!;
}
