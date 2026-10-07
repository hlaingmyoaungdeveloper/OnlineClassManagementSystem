using System;
using System.Collections.Generic;

namespace OnlineClassManagementSystem.Domain.models.User
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string? TelegramUsername { get; set; }
    }

    public class UserListResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = null!;
        public List<UserModel> Users { get; set; } = new();
    }

    public class UserEditRequestModel
    {
        public int UserId { get; set; }
    }

    public class UserEditResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = null!;
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? TelegramUsername { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
    }

    public class UserCreateRequestModel
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? TelegramUsername { get; set; }
    }

    public class UserCreateResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = null!;
    }

    public class UserPatchRequestModel
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? TelegramUsername { get; set; }
    }

    public class UserPatchResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = null!;
    }

    public class UserDeleteRequestModel
    {
        public int UserId { get; set; }
    }

    public class UserDeleteResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = null!;
    }
}
