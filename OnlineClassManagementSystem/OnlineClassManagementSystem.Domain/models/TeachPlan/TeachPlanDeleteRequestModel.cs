using System;

namespace OnlineClassManagementSystem.Domain.models.TeachPlan;

public class TeachPlanDeleteRequestModel
{
    public int Id { get; set; }
}

public class TeachPlanDeleteResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}
