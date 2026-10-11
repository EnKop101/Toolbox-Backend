using System;
using System.Collections.Generic;
using System.Text;
using Toolbox_Library.Enums;

namespace Toolbox_Library.Models
{
    internal class User
    {
        internal Guid Id { get; set; }
        internal string Username { get; set; }
        internal string Password { get; set; }
        internal UserRole Role { get; set; }
        internal string DisplayName { get; set; }
        internal DateOnly DateCreated { get; set; }
        internal UserStatus Status { get; set; }
    }
}
