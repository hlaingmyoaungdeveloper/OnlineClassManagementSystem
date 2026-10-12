using OnlineClassManagementSystem.Shared.models.Timetable;
using OnlineClassManagementSystem.Shared.models.SubClass;
using OnlineClassManagementSystem.Shared.models.User;
using OnlineClassManagementSystem.Shared.models.TeachPlan;

namespace OnlineClassManagementSystem.MvcApp.Services;

public class TimetableApiService
{
    private readonly HttpClient _httpClient;

    public TimetableApiService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("ApiClient");
    }

    // GET: All Timetables
    public async Task<TimetableListResponseModel?> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<TimetableListResponseModel>("api/Timetables");
    }

    // GET: Timetables by SubClassId
    public async Task<TimetableListByClassResponseModel?> GetByClassAsync(int subClassId)
    {
        return await _httpClient.GetFromJsonAsync<TimetableListByClassResponseModel>($"api/Timetables/club/{subClassId}");
    }

    // GET: Timetables by TeacherId
    public async Task<TimetableListByTeacherResponseModel?> GetByTeacherAsync(int teacherId)
    {
        return await _httpClient.GetFromJsonAsync<TimetableListByTeacherResponseModel>($"api/Timetables/teacher/{teacherId}");
    }

    // POST: Create Timetable
    public async Task<TimetableCreateResponseModel?> CreateTimetableAsync(TimetableCreateRequestModel requestModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Timetables", requestModel);
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<TimetableCreateResponseModel>();
            return errorResult ?? new TimetableCreateResponseModel { IsSuccess = false, Message = "Failed to create timetable" };
        }

        var result = await response.Content.ReadFromJsonAsync<TimetableCreateResponseModel>();
        return result;
    }

    // PATCH: Update Timetable
    public async Task<TimetablePatchResponseModel?> UpdateTimetableAsync(int id, TimetablePatchRequestModel model)
    {
        var response = await _httpClient.PatchAsJsonAsync($"api/Timetables/{id}", model);
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<TimetablePatchResponseModel>();
            return errorResult ?? new TimetablePatchResponseModel { IsSuccess = false, Message = "Failed to update timetable" };
        }

        var result = await response.Content.ReadFromJsonAsync<TimetablePatchResponseModel>();
        return result;
    }

    // DELETE: Delete Timetable
    public async Task<TimetableDeleteResponseModel?> DeleteTimetableAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/Timetables/{id}");
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<TimetableDeleteResponseModel>();
            return errorResult ?? new TimetableDeleteResponseModel { IsSuccess = false, Message = "Failed to delete timetable" };
        }

        var result = await response.Content.ReadFromJsonAsync<TimetableDeleteResponseModel>();
        return result;
    }

    // Helper: Load SubClasses for dropdowns
    public async Task<SubClassListResponseModel?> GetSubClassesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<SubClassListResponseModel>("api/SubClass");
        }
        catch
        {
            return null;
        }
    }

    // Helper: Load Teachers (Users) for dropdowns
    public async Task<UserListResponseModel?> GetUsersAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserListResponseModel>("api/User");
        }
        catch
        {
            return null;
        }
    }

    // Helper: Load TeachPlans for subjects
    public async Task<TeachPlanListResponseModel?> GetTeachPlansAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<TeachPlanListResponseModel>("api/TeachPlan");
        }
        catch
        {
            return null;
        }
    }
}
