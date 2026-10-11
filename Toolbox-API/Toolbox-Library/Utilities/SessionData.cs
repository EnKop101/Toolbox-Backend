using Toolbox_Library.Enums;

namespace Toolbox_Library.Utilities
{
    public class SessionData
    {
        public string UserId { get; set; }
        public UserRole Role { get; set; }

        public SessionData(string userId, UserRole role)
        {
            UserId = userId;
            Role = role;
        }
    }
}
