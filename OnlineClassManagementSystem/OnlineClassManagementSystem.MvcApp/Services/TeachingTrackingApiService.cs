using OnlineClassManagementSystem.Shared.models.TeachingTracking;
using OnlineClassManagementSystem.Shared.models.SubClass;
using OnlineClassManagementSystem.Shared.models.TeachPlan;
using OnlineClassManagementSystem.Shared.models.User;

namespace OnlineClassManagementSystem.MvcApp.Services;

public class TeachingTrackingApiService
{
    private readonly HttpClient _httpClient;

    public TeachingTrackingApiService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("ApiClient");
    }

    // GET: All TeachingTrackings
    public async Task<TeachingTrackingListResponseModel?> GetAllAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<TeachingTrackingListResponseModel>("api/TeachingTracking");
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch
        {
            return null;
        }
    }

    // GET: TeachingTracking by TrackId
    public async Task<TeachingTrackingEditResponseModel?> GetByIdAsync(int trackId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<TeachingTrackingEditResponseModel>($"api/TeachingTracking/{trackId}");
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch
        {
            return null;
        }
    }

    // POST: Create TeachingTracking
    public async Task<TeachingTrackingCreateResponseModel?> CreateTeachingTrackingAsync(TeachingTrackingCreateRequestModel requestModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/TeachingTracking", requestModel);
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<TeachingTrackingCreateResponseModel>();
            return errorResult ?? new TeachingTrackingCreateResponseModel { IsSuccess = false, Message = "Failed to create teaching tracking record." };
        }

        var result = await response.Content.ReadFromJsonAsync<TeachingTrackingCreateResponseModel>();
        return result;
    }

    // PATCH: Update TeachingTracking
    public async Task<bool> UpdateTeachingTrackingAsync(int trackId, TeachingTrackingPatchRequestModel model)
    {
        var response = await _httpClient.PatchAsJsonAsync($"api/TeachingTracking/{trackId}", model);
        return response.IsSuccessStatusCode;
    }

    // DELETE: Delete TeachingTracking
    public async Task<bool> DeleteTeachingTrackingAsync(int trackId)
    {
        var response = await _httpClient.DeleteAsync($"api/TeachingTracking/{trackId}");
        return response.IsSuccessStatusCode;
    }

    // Dropdown helpers
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
}
