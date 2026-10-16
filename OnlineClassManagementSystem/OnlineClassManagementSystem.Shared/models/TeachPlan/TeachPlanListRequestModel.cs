using System;
using System.Collections.Generic;

namespace OnlineClassManagementSystem.Shared.models.TeachPlan;

public class TeachPlanListRequestModel
{
    public int? PageNo { get; set; }
    public int? PageSize { get; set; }
    public string? Search { get; set; }
    public int? SubjectId { get; set; }
}

public class TeachPlanListResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public List<TeachPlanModel> TeachPlanList { get; set; } = new();

    public int TotalCount { get; set; }
    public int PageNo { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    public bool HasPreviousPage => PageNo > 1;
    public bool HasNextPage => PageNo < TotalPages;
}

public class TeachPlanModel
{
    public int Id { get; set; }

    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = null!;
    public string Topic { get; set; } = null!;
}
