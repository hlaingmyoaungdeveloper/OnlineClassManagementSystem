using OnlineClassManagementSystem.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.Domain.models;

public class SubClassListRequestModel
{

}

public class SubClassListResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public List<SubClassModel> SubClassList { get; set; } = null!;
}

public class SubClassModel
{
   // public int SubClassId { get; set; }

    public string ClassName { get; set; } = null!;

    public string Place { get; set; } = null!;

    public DateOnly? OpenDate { get; set; }

    public string OpenTime { get; set; } = null!;

    public int StudentLimit { get; set; }

    public int? StudentCount { get; set; }

    public bool IsDelete { get; set; }
}

