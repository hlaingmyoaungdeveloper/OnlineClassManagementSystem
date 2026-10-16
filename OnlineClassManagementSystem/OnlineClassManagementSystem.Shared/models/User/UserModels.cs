using System;
using System.Collections.Generic;

namespace OnlineClassManagementSystem.Shared.models.User
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

    public class UserListRequestModel
    {
        public int? PageNo { get; set; }
        public int? PageSize { get; set; }
        public string? Search { get; set; }
        public string? Role { get; set; }
    }

    public class UserListResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = null!;
        public List<UserModel> Users { get; set; } = new();

        public int TotalCount { get; set; }
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
        public bool HasPreviousPage => PageNo > 1;
        public bool HasNextPage => PageNo < TotalPages;
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
        public int? UserId { get; set; }
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
