using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using Toolbox_Library.Enums;

namespace Toolbox_Library.DTO
{
    public record UserDTO(Guid Id, string Username, UserRole Role, DateOnly DateCreated);
    public record LoginDTO(string Username, string Password);
    public record CreateUserDTO(Guid Id, string Username, string Password, UserRole Role);
}
