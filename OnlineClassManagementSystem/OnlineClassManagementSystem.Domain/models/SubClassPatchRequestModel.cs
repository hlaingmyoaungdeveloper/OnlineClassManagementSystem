using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.Domain.models;

public class SubClassPatchRequestModel
{
    public string ClassName { get; set; } = null!;

    public string Place { get; set; } = null!;

    public DateOnly OpenDate { get; set; }

    public string OpenTime { get; set; } = null!;

    public int StudentLimit { get; set; }

    public int? StudentCount { get; set; }
}

public class SubClassPatchResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
}
