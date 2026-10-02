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

    public int TeacherId { get; set; }

    public int ClassId { get; set; }

    public int SubjectId { get; set; }

    public string Topic { get; set; } = null!;

    public bool? IsCompleted { get; set; }

    public DateTime? CompletedDate { get; set; }
}
