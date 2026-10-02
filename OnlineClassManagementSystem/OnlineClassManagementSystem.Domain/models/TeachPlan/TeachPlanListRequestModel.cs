using System;
using System.Collections.Generic;

namespace OnlineClassManagementSystem.Domain.models.TeachPlan;

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

    public int TeacherId { get; set; }

    public int ClassId { get; set; }

    public int SubjectId { get; set; }

    public string Topic { get; set; } = null!;

    public bool? IsCompleted { get; set; }

    public DateTime? CompletedDate { get; set; }
}
