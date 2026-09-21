using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.Domain.models;

public class TimetableListByTeacherRequestModel
{
    public int TeacherId { get; set; }
}

public class TimetableListByTeacherResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public List<TimetableModel> TimetableList { get; set; }
}
