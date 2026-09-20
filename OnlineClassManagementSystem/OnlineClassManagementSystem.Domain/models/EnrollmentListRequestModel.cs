using System;
using System.Collections.Generic;

namespace OnlineClassManagementSystem.Domain.models;

public class EnrollmentListRequestModel
{
}

public class EnrollmentListResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public List<EnrollmentModel> EnrollmentList { get; set; } = null!;
}

public class EnrollmentModel
{
    public int EnrollmentId { get; set; }

    public int ClassId { get; set; }

    public string? ClassName { get; set; }

    public int StudentId { get; set; }

    public string? StudentName { get; set; }

    public DateTime? EnrollDate { get; set; }

    public bool IsDelete { get; set; }
}
