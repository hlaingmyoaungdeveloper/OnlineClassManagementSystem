using System;

namespace OnlineClassManagementSystem.Shared.models.TeachPlan;

public class TeachPlanDeleteRequestModel
{
    public int Id { get; set; }
}

public class TeachPlanDeleteResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}
