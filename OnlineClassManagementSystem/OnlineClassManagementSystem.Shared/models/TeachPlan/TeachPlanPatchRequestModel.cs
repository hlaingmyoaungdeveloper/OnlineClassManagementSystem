using System;

namespace OnlineClassManagementSystem.Domain.models.TeachPlan;

public class TeachPlanPatchRequestModel
{
    public int? SubjectId { get; set; }

    public string? Topic { get; set; }
}

public class TeachPlanPatchResponseModel
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}
