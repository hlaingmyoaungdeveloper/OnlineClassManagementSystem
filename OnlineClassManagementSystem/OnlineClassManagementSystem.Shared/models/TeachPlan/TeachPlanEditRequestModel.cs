using System;

namespace OnlineClassManagementSystem.Domain.models.TeachPlan;

public class TeachPlanEditRequestModel
{
    public int Id { get; set; }
}

public class TeachPlanEditResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;

    public int Id { get; set; }

    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = null!;
    public string Topic { get; set; } = null!;

}
