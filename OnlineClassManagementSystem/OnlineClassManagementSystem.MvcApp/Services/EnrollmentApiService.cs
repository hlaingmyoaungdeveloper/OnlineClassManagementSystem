using OnlineClassManagementSystem.Shared.models.Enrollment;
using OnlineClassManagementSystem.Shared.models.SubClass;
using OnlineClassManagementSystem.Shared.models.User;

namespace OnlineClassManagementSystem.MvcApp.Services;

public class EnrollmentApiService
{
    private readonly HttpClient _httpClient;

    public EnrollmentApiService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("ApiClient");
    }

    // GET: All Enrollments
    public async Task<EnrollmentListResponseModel?> GetAllAsync(EnrollmentListRequestModel? requestModel = null)
    {
        try
        {
            var query = new List<string>();
            if (requestModel?.PageNo.HasValue == true) query.Add($"pageNo={requestModel.PageNo.Value}");
            if (requestModel?.PageSize.HasValue == true) query.Add($"pageSize={requestModel.PageSize.Value}");
            if (!string.IsNullOrWhiteSpace(requestModel?.Search)) query.Add($"search={Uri.EscapeDataString(requestModel.Search.Trim())}");
            if (!string.IsNullOrWhiteSpace(requestModel?.Status) && requestModel.Status.ToLower() != "all") query.Add($"status={Uri.EscapeDataString(requestModel.Status.Trim())}");
            if (requestModel?.ClassId.HasValue == true) query.Add($"classId={requestModel.ClassId.Value}");

            var url = query.Count > 0 ? $"api/Enrollment?{string.Join("&", query)}" : "api/Enrollment";
            return await _httpClient.GetFromJsonAsync<EnrollmentListResponseModel>(url);
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

    // GET: Enrollment by Id
    public async Task<EnrollmentEditResponseModel?> GetByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<EnrollmentEditResponseModel>($"api/Enrollment/{id}");
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

    // POST: Create Enrollment
    public async Task<EnrollmentCreateResponseModel?> CreateEnrollmentAsync(EnrollmentCreateRequestModel requestModel)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Enrollment", requestModel);
            var result = await response.Content.ReadFromJsonAsync<EnrollmentCreateResponseModel>();
            return result ?? new EnrollmentCreateResponseModel
            {
                IsSuccess = response.IsSuccessStatusCode,
                Message = response.IsSuccessStatusCode ? "Enrollment created successfully." : "Failed to create enrollment."
            };
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

    // PATCH: Update Enrollment (Class, Student, Date)
    public async Task<EnrollmentPatchResponseModel?> UpdateEnrollmentAsync(int id, EnrollmentPatchRequestModel model)
    {
        try
        {
            var response = await _httpClient.PatchAsJsonAsync($"api/Enrollment/{id}", model);
            var result = await response.Content.ReadFromJsonAsync<EnrollmentPatchResponseModel>();
            return result ?? new EnrollmentPatchResponseModel
            {
                IsSuccess = response.IsSuccessStatusCode,
                Message = response.IsSuccessStatusCode ? "Enrollment updated successfully." : "Failed to update enrollment."
            };
        }
        catch (Exception ex)
        {
            return new EnrollmentPatchResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    // PATCH: Update Enrollment Status (confirm, pending, canceled)
    public async Task<EnrollmentUpdateStatusResponseModel?> UpdateStatusAsync(EnrollmentUpdateStatusResquestModel model)
    {
        try
        {
            var response = await _httpClient.PatchAsJsonAsync("api/Enrollment", model);
            var result = await response.Content.ReadFromJsonAsync<EnrollmentUpdateStatusResponseModel>();
            return result ?? new EnrollmentUpdateStatusResponseModel
            {
                IsSuccess = response.IsSuccessStatusCode,
                Message = response.IsSuccessStatusCode ? "Enrollment status updated." : "Failed to update enrollment status."
            };
        }
        catch (Exception ex)
        {
            return new EnrollmentUpdateStatusResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    // Dropdown helpers
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
