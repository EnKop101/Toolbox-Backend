using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using Toolbox_Library.Enums;
using Toolbox_Library.Models;

namespace Toolbox_Library.DTO
{
    public record UserDTO(Guid Id, string Username, UserRole Role, DateOnly DateCreated, DateOnly LastUpdated)
    {
        public UserDTO(User user) : this(
            user.Id,
            user.Username,
            user.Role,
            user.DateCreated,
            user.LastUpdated
        ){}
    }
    public record LoginDTO(string Username, string Password);
    public record CreateUserDTO(string Username, string Password, string DisplayName);
    public record UpdateUserDTO(Guid Id, string Username, string Password, UserRole Role);
    public record ChangePassword(Guid Id, string Username, string OldPassword, string NewPassword);
}
