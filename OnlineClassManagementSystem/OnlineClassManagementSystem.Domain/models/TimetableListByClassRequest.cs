using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.Domain.models;

public class TimetableListByClassRequest
{
    public int SubClassId { get; set; }
}

public class TimetableListByClassResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public List<TimetableModel> TimetableList { get; set; }
}
