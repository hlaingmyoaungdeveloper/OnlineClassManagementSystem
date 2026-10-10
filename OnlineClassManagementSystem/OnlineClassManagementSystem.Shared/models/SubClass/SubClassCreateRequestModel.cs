using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.Shared.models.SubClass;

public class SubClassCreateRequestModel
{
    public string ClassName { get; set; } = null!;

    public string Place { get; set; } = null!;

    public DateOnly OpenDate { get; set; }

    public string OpenTime { get; set; } = null!;

    public int StudentLimit { get; set; }

}

public class SubClassCreateResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }

    public static implicit operator bool(SubClassCreateResponseModel v)
    {
        throw new NotImplementedException();
    }
}
