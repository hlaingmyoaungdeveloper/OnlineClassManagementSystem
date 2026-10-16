using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.Shared.models.SubClass;

public class SubClassListRequestModel
{
    public int? PageNo { get; set; }
    public int? PageSize { get; set; }
    public string? Search { get; set; }
    public string? Status { get; set; }
}

public class SubClassListResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public List<SubClassModel> SubClassList { get; set; } = null!;

    public int TotalCount { get; set; }
    public int PageNo { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    public bool HasPreviousPage => PageNo > 1;
    public bool HasNextPage => PageNo < TotalPages;

    public int TotalClasses { get; set; }
    public int TotalEnrolled { get; set; }
    public int TotalCapacity { get; set; }
    public int AvailableSeats { get; set; }
}

public class SubClassModel
{
    public int SubClassId { get; set; }

    public string ClassName { get; set; } = null!;

    public string Place { get; set; } = null!;

    public DateOnly? OpenDate { get; set; }

    public string OpenTime { get; set; } = null!;

    public int StudentLimit { get; set; }

    public int? StudentCount { get; set; }
}

