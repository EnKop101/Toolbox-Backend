using System;
using System.Collections.Generic;
using System.Text;
using Toolbox_Library.Enums;

namespace Toolbox_Library.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public string DisplayName { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Pending;
        public DateOnly DateCreated { get; set; }
        public DateOnly LastUpdated { get; set; }
    }
}
