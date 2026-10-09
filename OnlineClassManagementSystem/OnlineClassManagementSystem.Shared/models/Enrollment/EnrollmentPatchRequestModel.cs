using System;

namespace OnlineClassManagementSystem.Domain.models.Enrollment;

public class EnrollmentPatchRequestModel
{
    public int? ClassId { get; set; }

    public int? StudentId { get; set; }

    public DateTime? EnrollDate { get; set; }
}

public class EnrollmentPatchResponseModel
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}
