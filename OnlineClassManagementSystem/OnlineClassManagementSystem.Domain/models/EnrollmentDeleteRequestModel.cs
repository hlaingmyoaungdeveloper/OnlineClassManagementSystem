using System;

namespace OnlineClassManagementSystem.Domain.models;

public class EnrollmentDeleteRequestModel
{
    public int EnrollmentId { get; set; }
}

public class EnrollmentDeleteResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}
