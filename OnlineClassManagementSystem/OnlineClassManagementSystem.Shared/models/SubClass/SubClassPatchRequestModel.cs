using System;

namespace OnlineClassManagementSystem.Domain.models.SubClass;

public class SubClassPatchRequestModel
{
    public string? ClassName { get; set; }

    public string? Place { get; set; }

    public DateOnly? OpenDate { get; set; }

    public string? OpenTime { get; set; }

    public int? StudentLimit { get; set; }

}

public class SubClassPatchResponseModel
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}