using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.Domain.models;

public class EnrollmentUpdateStatusResquestModel
{
    public int EnrollmentId {  get; set; }
    public string Status {  get; set; }
}

public class EnrollmentUpdateStatusResponseModel
{
    public bool IsSuccess {  get; set; }
    public string Message { get; set; }
}
