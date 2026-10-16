using System;
using System.Collections.Generic;

namespace OnlineClassManagementSystem.Shared.models.Enrollment;

public class EnrollmentListRequestModel
{
    public int? PageNo { get; set; }
    public int? PageSize { get; set; }
    public string? Search { get; set; }
    public int? ClassId { get; set; }
    public string? Status { get; set; }
}

public class EnrollmentListResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public List<EnrollmentModel> EnrollmentList { get; set; } = new();

    public int TotalCount { get; set; }
    public int PageNo { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    public bool HasPreviousPage => PageNo > 1;
    public bool HasNextPage => PageNo < TotalPages;

    public int ConfirmedCount { get; set; }
    public int PendingCount { get; set; }
    public int CanceledCount { get; set; }
}

public class EnrollmentModel
{
    public int EnrollmentId { get; set; }

    public int ClassId { get; set; }

    public string? ClassName { get; set; }

    public int StudentId { get; set; }

    public string? StudentName { get; set; }

    public string? Status { get; set; }
    public DateTime? EnrollDate { get; set; }
}
