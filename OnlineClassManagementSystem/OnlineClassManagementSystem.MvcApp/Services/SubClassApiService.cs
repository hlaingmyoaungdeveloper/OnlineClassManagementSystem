using OnlineClassManagementSystem.Shared.models.SubClass;
using OnlineClassManagementSystem.Shared.models.Enrollment;
using OnlineClassManagementSystem.Shared.models.User;

namespace OnlineClassManagementSystem.MvcApp.Services;

public class SubClassApiService
{
    private readonly HttpClient _httpClient;

    public SubClassApiService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("ApiClient");
    }

    public async Task<SubClassListResponseModel?> GetAllAsync(SubClassListRequestModel? requestModel = null)
    {
        var query = new List<string>();
        if (requestModel?.PageNo.HasValue == true) query.Add($"pageNo={requestModel.PageNo.Value}");
        if (requestModel?.PageSize.HasValue == true) query.Add($"pageSize={requestModel.PageSize.Value}");
        if (!string.IsNullOrWhiteSpace(requestModel?.Search)) query.Add($"search={Uri.EscapeDataString(requestModel.Search.Trim())}");
        if (!string.IsNullOrWhiteSpace(requestModel?.Status) && requestModel.Status.ToLower() != "all") query.Add($"status={Uri.EscapeDataString(requestModel.Status.Trim())}");

        var url = query.Count > 0 ? $"api/SubClass?{string.Join("&", query)}" : "api/SubClass";
        return await _httpClient.GetFromJsonAsync<SubClassListResponseModel>(url);
    }

    public async Task<SubClassCreateResponseModel?> CreateSubClassAsync(SubClassCreateRequestModel requestModel)
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
    public async Task<SubClassEditResponseModel?> GetSubClassAsync(int id)
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

    // GET: Load Users/Students
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

    // POST: Create User/Student
    public async Task<UserCreateResponseModel?> CreateUserAsync(UserCreateRequestModel requestModel)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/User", requestModel);
            var result = await response.Content.ReadFromJsonAsync<UserCreateResponseModel>();
            return result;
        }
        catch (Exception ex)
        {
            return new UserCreateResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    // POST: Enroll Student
    public async Task<EnrollmentCreateResponseModel?> EnrollStudentAsync(EnrollmentCreateRequestModel requestModel)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Enrollment", requestModel);
            var result = await response.Content.ReadFromJsonAsync<EnrollmentCreateResponseModel>();
            return result;
        }
        catch (Exception ex)
        {
            return new EnrollmentCreateResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }
}

