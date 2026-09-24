using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.Domain.models.SubClass;

public class SubClassDeleteRequestModel
{
    public int SubClassId { get; set; }
}

public class SubClassDeleteResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
}
