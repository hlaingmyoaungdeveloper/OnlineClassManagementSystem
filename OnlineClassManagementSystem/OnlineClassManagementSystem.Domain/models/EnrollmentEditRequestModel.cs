using System;

namespace OnlineClassManagementSystem.Domain.models;

public class EnrollmentEditRequestModel
{
    public int EnrollmentId { get; set; }
}

public class EnrollmentEditResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public int EnrollmentId { get; set; }
    public int ClassId { get; set; }
    public string? ClassName { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public DateTime? EnrollDate { get; set; }
}
