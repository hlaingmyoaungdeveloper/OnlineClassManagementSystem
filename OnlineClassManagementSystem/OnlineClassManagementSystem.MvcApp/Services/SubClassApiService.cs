using OnlineClassManagementSystem.Domain.models.SubClass;

namespace OnlineClassManagementSystem.MvcApp.Services;

public class SubClassApiService
{
    private readonly HttpClient _httpClient;

    public SubClassApiService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("ApiClient");
    }

    public async Task<SubClassListResponseModel> GetAllAsync()
    {
        return await _httpClient
          .GetFromJsonAsync<SubClassListResponseModel>(
              "api/SubClass");
    }
}
