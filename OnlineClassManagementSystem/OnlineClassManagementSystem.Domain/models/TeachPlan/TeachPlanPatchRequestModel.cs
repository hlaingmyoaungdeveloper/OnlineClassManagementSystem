using System;

namespace OnlineClassManagementSystem.Domain.models.TeachPlan;

public class TeachPlanPatchRequestModel
{
    public int? TeacherId { get; set; }

    public int? ClassId { get; set; }

    public int? SubjectId { get; set; }

    public string? Topic { get; set; }

    public bool? IsCompleted { get; set; }

    public DateTime? CompletedDate { get; set; }
}

public class TeachPlanPatchResponseModel
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}
