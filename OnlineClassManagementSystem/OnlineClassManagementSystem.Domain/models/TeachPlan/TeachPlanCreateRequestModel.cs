using System;

namespace OnlineClassManagementSystem.Domain.models.TeachPlan;

public class TeachPlanCreateRequestModel
{
    public int TeacherId { get; set; }

    public int ClassId { get; set; }

    public int SubjectId { get; set; }

    public string Topic { get; set; } = null!;
}

public class TeachPlanCreateResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}
