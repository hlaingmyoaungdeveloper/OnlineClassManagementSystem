using System;

namespace OnlineClassManagementSystem.Domain.models;

public class EnrollmentCreateRequestModel
{
    public int ClassId { get; set; }

    public int StudentId { get; set; }

    public DateTime? EnrollDate { get; set; }
}

public class EnrollmentCreateResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}
