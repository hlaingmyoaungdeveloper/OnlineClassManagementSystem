using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.Domain.models.Timetable;

public class TimetableCreateRequestModel
{
    public int SubClassId { get; set; }

    public int TeacherId { get; set; }

    public int SubjectId { get; set; }

    public string DayOfWeek { get; set; } = null!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string Mode { get; set; } = null!;
}

public class TimetableCreateResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
}