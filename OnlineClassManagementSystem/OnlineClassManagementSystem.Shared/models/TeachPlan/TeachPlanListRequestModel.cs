using System;
using System.Collections.Generic;

namespace OnlineClassManagementSystem.Shared.models.TeachPlan;

public class TeachPlanListRequestModel
{

}

public class TeachPlanListResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public List<TeachPlanModel> TeachPlanList { get; set; } = null!;
}

public class TeachPlanModel
{
    public int Id { get; set; }

    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = null!;
    public string Topic { get; set; } = null!;
}
