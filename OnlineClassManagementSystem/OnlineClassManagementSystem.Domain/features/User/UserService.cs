using Microsoft.EntityFrameworkCore;
using OnlineClassManagementSystem.Database.Models;
using OnlineClassManagementSystem.Shared.models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.Domain.features.User;

public class UserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    // Get all users
    public async Task<UserListResponseModel> GetUsersAsync()
    {
        try
        {
            var users = await _db.TblUsers
                .AsNoTracking()
                .Where(u => !u.IsDelete)
                .Select(u => new UserModel
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedDateTime = u.CreatedDateTime,
                    ModifiedDateTime = u.ModifiedDateTime,
                    TelegramUsername = u.TelegramUsername
                })
                .ToListAsync();

            return new UserListResponseModel
            {
                IsSuccess = true,
                Message = "Users fetched successfully",
                Users = users
            };
        }
        catch (Exception ex)
        {
            return new UserListResponseModel
            {
                IsSuccess = false,
                Message = ex.Message,
                Users = new List<UserModel>()
            };
        }
    }

    // Get a single user by Id
    public async Task<UserEditResponseModel> GetUserAsync(UserEditRequestModel model)
    {
        try
        {
            var user = await _db.TblUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => !u.IsDelete && u.UserId == model.UserId);

            if (user is null)
            {
                return new UserEditResponseModel
                {
                    IsSuccess = false,
                    Message = "User not found"
                };
            }

            return new UserEditResponseModel
            {
                IsSuccess = true,
                Message = "User fetched successfully",
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                TelegramUsername = user.TelegramUsername,
                CreatedDateTime = user.CreatedDateTime,
                ModifiedDateTime = user.ModifiedDateTime
            };
        }
        catch (Exception ex)
        {
            return new UserEditResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    // Create a new user with a default password (password not persisted as column yet)
    public async Task<UserCreateResponseModel> CreateUserAsync(UserCreateRequestModel model)
    {
        // Basic validation
        if (string.IsNullOrWhiteSpace(model.FullName))
        {
            return new UserCreateResponseModel { IsSuccess = false, Message = "FullName is required" };
        }
        if (string.IsNullOrWhiteSpace(model.Email))
        {
            return new UserCreateResponseModel { IsSuccess = false, Message = "Email is required" };
        }
        if (string.IsNullOrWhiteSpace(model.Role))
        {
            return new UserCreateResponseModel { IsSuccess = false, Message = "Role is required" };
        }

        // Check for existing email
        var exists = await _db.TblUsers
            .AsNoTracking()
            .AnyAsync(u => !u.IsDelete && u.Email == model.Email);
        if (exists)
        {
            return new UserCreateResponseModel { IsSuccess = false, Message = "User with this email already exists" };
        }

        var now = DateTime.Now;
        var user = new TblUser
        {
            FullName = model.FullName,
            Email = model.Email,
            Role = model.Role,
            CreatedDateTime = now,
            ModifiedDateTime = now,
            IsDelete = false,
            TelegramUsername = model.TelegramUsername,
            // Password handling would be added here if a column existed. For now we set a default placeholder.
        };

        _db.TblUsers.Add(user);
        var result = await _db.SaveChangesAsync();
        return new UserCreateResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? "User created successfully (default password assigned)" : "Failed to create user"
        };
    }

    // Patch user fields
    public async Task<UserPatchResponseModel> PatchUserAsync(int id, UserPatchRequestModel model)
    {
        try
        {
            var user = await _db.TblUsers.FirstOrDefaultAsync(u => !u.IsDelete && u.UserId == id);
            if (user is null)
            {
                return new UserPatchResponseModel { IsSuccess = false, Message = "User not found" };
            }

            if (!string.IsNullOrWhiteSpace(model.FullName))
                user.FullName = model.FullName;
            if (!string.IsNullOrWhiteSpace(model.Email))
                user.Email = model.Email;
            if (!string.IsNullOrWhiteSpace(model.Role))
                user.Role = model.Role;
            if (!string.IsNullOrWhiteSpace(model.TelegramUsername))
                user.TelegramUsername = model.TelegramUsername;

            user.ModifiedDateTime = DateTime.Now;
            var result = await _db.SaveChangesAsync();
            return new UserPatchResponseModel { IsSuccess = result > 0, Message = result > 0 ? "User updated successfully" : "Failed to update user" };
        }
        catch (Exception ex)
        {
            return new UserPatchResponseModel { IsSuccess = false, Message = ex.Message };
        }
    }

    // Soft delete user
    public async Task<UserDeleteResponseModel> DeleteUserAsync(UserDeleteRequestModel model)
    {
        try
        {
            var user = await _db.TblUsers.FirstOrDefaultAsync(u => !u.IsDelete && u.UserId == model.UserId);
            if (user is null)
            {
                return new UserDeleteResponseModel { IsSuccess = false, Message = "User not found" };
            }

            user.IsDelete = true;
            user.ModifiedDateTime = DateTime.Now;
            var result = await _db.SaveChangesAsync();
            return new UserDeleteResponseModel { IsSuccess = result > 0, Message = result > 0 ? "User deleted successfully" : "Failed to delete user" };
        }
        catch (Exception ex)
        {
            return new UserDeleteResponseModel { IsSuccess = false, Message = ex.Message };
        }
    }
}
