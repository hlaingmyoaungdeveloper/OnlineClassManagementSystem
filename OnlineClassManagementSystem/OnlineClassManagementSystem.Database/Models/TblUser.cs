using System;
using System.Collections.Generic;

namespace OnlineClassManagementSystem.Database.Models;

public partial class TblUser
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime CreatedDateTime { get; set; }

    public DateTime ModifiedDateTime { get; set; }

    public bool IsDelete { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public string? TelegramUsername { get; set; }

    public virtual ICollection<TblEnrollment> TblEnrollments { get; set; } = new List<TblEnrollment>();

    public virtual ICollection<TblTimeTable> TblTimeTables { get; set; } = new List<TblTimeTable>();
}
