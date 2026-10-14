using OnlineClassManagementSystem.Shared.models.User;

namespace OnlineClassManagementSystem.MvcApp.Services;

public class UserApiService
{
    private readonly HttpClient _httpClient;

    public UserApiService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("ApiClient");
    }

    // GET: All Users
    public async Task<UserListResponseModel?> GetUsersAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserListResponseModel>("api/User");
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

    // GET: User by Id
    public async Task<UserEditResponseModel?> GetUserAsync(int userId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserEditResponseModel>($"api/User/{userId}");
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

    // POST: Create User
    public async Task<UserCreateResponseModel?> CreateUserAsync(UserCreateRequestModel requestModel)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/User", requestModel);
            var result = await response.Content.ReadFromJsonAsync<UserCreateResponseModel>();
            return result ?? new UserCreateResponseModel
            {
                IsSuccess = response.IsSuccessStatusCode,
                Message = response.IsSuccessStatusCode ? "User created successfully." : "Failed to create user."
            };
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

    // PATCH: Update User
    public async Task<UserPatchResponseModel?> UpdateUserAsync(int id, UserPatchRequestModel model)
    {
        try
        {
            var response = await _httpClient.PatchAsJsonAsync($"api/User/{id}", model);
            var result = await response.Content.ReadFromJsonAsync<UserPatchResponseModel>();
            return result ?? new UserPatchResponseModel
            {
                IsSuccess = response.IsSuccessStatusCode,
                Message = response.IsSuccessStatusCode ? "User updated successfully." : "Failed to update user."
            };
        }
        catch (Exception ex)
        {
            return new UserPatchResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    // DELETE: Delete User
    public async Task<UserDeleteResponseModel?> DeleteUserAsync(int userId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/User/{userId}");
            var result = await response.Content.ReadFromJsonAsync<UserDeleteResponseModel>();
            return result ?? new UserDeleteResponseModel
            {
                IsSuccess = response.IsSuccessStatusCode,
                Message = response.IsSuccessStatusCode ? "User deleted successfully." : "Failed to delete user."
            };
        }
        catch (Exception ex)
        {
            return new UserDeleteResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }
}
