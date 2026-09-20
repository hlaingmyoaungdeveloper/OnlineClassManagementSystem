using System;

namespace OnlineClassManagementSystem.Domain.models;

public class SubClassPatchRequestModel
{
    // ? ထည့်သွင်းထားသဖြင့် Frontend မှ မပို့လိုက်သော Data များအတွက် Error မတက်တော့ပါ
    public string? ClassName { get; set; }

    public string? Place { get; set; }

    public DateOnly? OpenDate { get; set; }

    public string? OpenTime { get; set; }

    public int? StudentLimit { get; set; }

    // မှတ်ချက် - StudentCount အား Admin မှ Manual ပြင်ခွင့်ပြုမည်ဆိုပါက ဆက်ထားနိုင်ပါသည်။
    public int? StudentCount { get; set; }
}

public class SubClassPatchResponseModel
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}