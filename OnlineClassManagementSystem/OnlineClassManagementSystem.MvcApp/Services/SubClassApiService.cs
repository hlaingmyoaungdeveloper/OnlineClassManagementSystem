using OnlineClassManagementSystem.Shared.models.SubClass;

namespace OnlineClassManagementSystem.MvcApp.Services;

public class SubClassApiService
{
    private readonly HttpClient _httpClient;

    public SubClassApiService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("ApiClient");
    }

    public async Task<SubClassListResponseModel> GetAllAsync(SubClassListRequestModel requestModel)
    {
        return await _httpClient
          .GetFromJsonAsync<SubClassListResponseModel>(
              "api/SubClass");
    }

    public async Task<SubClassCreateResponseModel> CreateSubClassAsync(SubClassCreateRequestModel requestModel)
    {
        var response = await _httpClient
            .PostAsJsonAsync("api/SubClass",
                                   requestModel);
        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<SubClassCreateResponseModel>();

        return result;
    }
    // GET: SubClass details by id
    public async Task<SubClassEditResponseModel> GetSubClassAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<SubClassEditResponseModel>($"api/SubClass/{id}");
    }

    // PATCH: Update SubClass
    public async Task<bool> UpdateSubClassAsync(SubClassEditRequestModel model)
    {
        var patchModel = new SubClassPatchRequestModel
        {
            ClassName = model.ClassName,
            Place = model.Place,
            OpenDate = model.OpenDate,
            OpenTime = model.OpenTime,
            StudentLimit = model.StudentLimit
        };
        var response = await _httpClient.PatchAsJsonAsync($"api/SubClass/{model.SubClassId}", patchModel);
        return response.IsSuccessStatusCode;
    }

    // DELETE: Delete SubClass
    public async Task<bool> DeleteSubClassAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/SubClass/{id}");
        return response.IsSuccessStatusCode;
    }
}

