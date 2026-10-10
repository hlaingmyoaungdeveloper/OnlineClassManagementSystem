using System;

namespace OnlineClassManagementSystem.Shared.models.TeachPlan;

public class TeachPlanCreateRequestModel
{
    public int SubjectId { get; set; }

    public string Topic { get; set; } = null!;

}

public class TeachPlanCreateResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}
