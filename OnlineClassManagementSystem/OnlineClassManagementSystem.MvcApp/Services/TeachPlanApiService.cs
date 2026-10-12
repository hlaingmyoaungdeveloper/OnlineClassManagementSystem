using OnlineClassManagementSystem.Shared.models.TeachPlan;

namespace OnlineClassManagementSystem.MvcApp.Services;

public class TeachPlanApiService
{
    private readonly HttpClient _httpClient;

    public TeachPlanApiService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("ApiClient");
    }

    // GET: All TeachPlans
    public async Task<TeachPlanListResponseModel?> GetAllAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<TeachPlanListResponseModel>("api/TeachPlan");
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

    // GET: TeachPlan by Id
    public async Task<TeachPlanEditResponseModel?> GetByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<TeachPlanEditResponseModel>($"api/TeachPlan/{id}");
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

    // POST: Create TeachPlan
    public async Task<TeachPlanCreateResponseModel?> CreateTeachPlanAsync(TeachPlanCreateRequestModel requestModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/TeachPlan", requestModel);
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<TeachPlanCreateResponseModel>();
            return errorResult ?? new TeachPlanCreateResponseModel { IsSuccess = false, Message = "Failed to create teach plan." };
        }

        var result = await response.Content.ReadFromJsonAsync<TeachPlanCreateResponseModel>();
        return result;
    }

    // PATCH: Update TeachPlan
    public async Task<bool> UpdateTeachPlanAsync(int id, TeachPlanPatchRequestModel model)
    {
        var response = await _httpClient.PatchAsJsonAsync($"api/TeachPlan/{id}", model);
        return response.IsSuccessStatusCode;
    }

    // DELETE: Delete TeachPlan
    public async Task<bool> DeleteTeachPlanAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/TeachPlan/{id}");
        return response.IsSuccessStatusCode;
    }
}
