using System;

namespace OnlineClassManagementSystem.Shared.models.Enrollment;

public class SubClassEnrollRequestModel
{
    public int ClassId { get; set; }
    public bool IsNewStudent { get; set; }

    public int? StudentId { get; set; }

    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? TelegramUsername { get; set; }

    public DateTime? EnrollDate { get; set; }
}
