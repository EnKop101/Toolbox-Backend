using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using Toolbox_Library.Enums;

namespace Toolbox_Library.DTO
{
    public record UserDTO(Guid Id, string Username, UserRole Role, DateOnly DateCreated, DateOnly LastUpdated);
    public record LoginDTO(string Username, string Password);
    public record CreateUserDTO(string Username, string Password, string DisplayName, UserRole Role);
    public record UpdateUserDTO(Guid Id, string Username, string Password, UserRole Role);
    public record ChangePassword(Guid Id, string Username, string OldPassword, string NewPassword);
}
