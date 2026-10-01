using OnlineClassManagementSystem.MvcApp.Services.ApiModels;
using System.Net.Http.Json;
using System.Text.Json;

namespace OnlineClassManagementSystem.MvcApp.Services;

/// <summary>
/// Typed HTTP Client that encapsulates all calls to the OCMS Web API.
/// Injected via IHttpClientFactory — controllers never touch HttpClient directly.
/// </summary>
public class OcmsApiService
{
    private readonly HttpClient _http;
    private readonly ILogger<OcmsApiService> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OcmsApiService(HttpClient http, ILogger<OcmsApiService> logger)
    {
        _http = http;
        _logger = logger;
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  SubClass (Classes) Endpoints
    // ═══════════════════════════════════════════════════════════════════════

    public async Task<SubClassListApiResponse> GetAllSubClassesAsync()
    {
        try
        {
            var result = await _http.GetFromJsonAsync<SubClassListApiResponse>(
                "api/SubClass", _jsonOptions);
            return result ?? new SubClassListApiResponse { IsSuccess = false, Message = "Empty response from API." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching SubClasses");
            return new SubClassListApiResponse { IsSuccess = false, Message = ex.Message };
        }
    }

    public async Task<SubClassEditApiResponse> GetSubClassByIdAsync(int id)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<SubClassEditApiResponse>(
                $"api/SubClass/{id}", _jsonOptions);
            return result ?? new SubClassEditApiResponse { IsSuccess = false, Message = "Not found." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching SubClass {Id}", id);
            return new SubClassEditApiResponse { IsSuccess = false, Message = ex.Message };
        }
    }

    public async Task<ApiBaseResponse> CreateSubClassAsync(SubClassCreateApiRequest request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/SubClass", request);
            var result = await response.Content.ReadFromJsonAsync<ApiBaseResponse>(_jsonOptions);
            return result ?? new ApiBaseResponse { IsSuccess = false, Message = "Empty response." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating SubClass");
            return new ApiBaseResponse { IsSuccess = false, Message = ex.Message };
        }
    }

    public async Task<ApiBaseResponse> PatchSubClassAsync(int id, SubClassPatchApiRequest request)
    {
        try
        {
            var response = await _http.PatchAsJsonAsync($"api/SubClass/{id}", request);
            var result = await response.Content.ReadFromJsonAsync<ApiBaseResponse>(_jsonOptions);
            return result ?? new ApiBaseResponse { IsSuccess = false, Message = "Empty response." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error patching SubClass {Id}", id);
            return new ApiBaseResponse { IsSuccess = false, Message = ex.Message };
        }
    }

    public async Task<ApiBaseResponse> DeleteSubClassAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/SubClass/{id}");
            var result = await response.Content.ReadFromJsonAsync<ApiBaseResponse>(_jsonOptions);
            return result ?? new ApiBaseResponse { IsSuccess = false, Message = "Empty response." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting SubClass {Id}", id);
            return new ApiBaseResponse { IsSuccess = false, Message = ex.Message };
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  Enrollment Endpoints
    // ═══════════════════════════════════════════════════════════════════════

    public async Task<EnrollmentListApiResponse> GetAllEnrollmentsAsync()
    {
        try
        {
            var result = await _http.GetFromJsonAsync<EnrollmentListApiResponse>(
                "api/Enrollment", _jsonOptions);
            return result ?? new EnrollmentListApiResponse { IsSuccess = false, Message = "Empty response." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Enrollments");
            return new EnrollmentListApiResponse { IsSuccess = false, Message = ex.Message };
        }
    }

    public async Task<ApiBaseResponse> CreateEnrollmentAsync(EnrollmentCreateApiRequest request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/Enrollment", request);
            var result = await response.Content.ReadFromJsonAsync<ApiBaseResponse>(_jsonOptions);
            return result ?? new ApiBaseResponse { IsSuccess = false, Message = "Empty response." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Enrollment");
            return new ApiBaseResponse { IsSuccess = false, Message = ex.Message };
        }
    }

    public async Task<ApiBaseResponse> UpdateEnrollmentStatusAsync(EnrollmentUpdateStatusApiRequest request)
    {
        try
        {
            var response = await _http.PatchAsJsonAsync("api/Enrollment", request);
            var result = await response.Content.ReadFromJsonAsync<ApiBaseResponse>(_jsonOptions);
            return result ?? new ApiBaseResponse { IsSuccess = false, Message = "Empty response." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating enrollment status");
            return new ApiBaseResponse { IsSuccess = false, Message = ex.Message };
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  Timetable Endpoints
    // ═══════════════════════════════════════════════════════════════════════

    public async Task<TimetableListApiResponse> GetAllTimetablesAsync()
    {
        try
        {
            var result = await _http.GetFromJsonAsync<TimetableListApiResponse>(
                "api/Timetables", _jsonOptions);
            return result ?? new TimetableListApiResponse { IsSuccess = false, Message = "Empty response." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Timetables");
            return new TimetableListApiResponse { IsSuccess = false, Message = ex.Message };
        }
    }

    public async Task<TimetableListApiResponse> GetTimetablesByTeacherAsync(int teacherId)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<TimetableListApiResponse>(
                $"api/Timetables/teacher/{teacherId}", _jsonOptions);
            return result ?? new TimetableListApiResponse { IsSuccess = false, Message = "Empty response." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Timetables for teacher {Id}", teacherId);
            return new TimetableListApiResponse { IsSuccess = false, Message = ex.Message };
        }
    }

    public async Task<TimetableListApiResponse> GetTimetablesByClassAsync(int subClassId)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<TimetableListApiResponse>(
                $"api/Timetables/club/{subClassId}", _jsonOptions);
            return result ?? new TimetableListApiResponse { IsSuccess = false, Message = "Empty response." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Timetables for class {Id}", subClassId);
            return new TimetableListApiResponse { IsSuccess = false, Message = ex.Message };
        }
    }
}
