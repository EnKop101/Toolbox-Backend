using Microsoft.AspNetCore.Mvc;
using Toolbox_Library.Enums;
using Toolbox_Library.Utilities;

namespace Toolbox_API.Extension
{
    public class BaseController : ControllerBase
    {
        protected SessionData Session =>
            (SessionData)HttpContext.Items["SessionData"]!;

        protected Guid CurrentUserId => Guid.Parse(Session.UserId);
        protected UserRole CurrentUserRole => Session.Role;
    }
}
