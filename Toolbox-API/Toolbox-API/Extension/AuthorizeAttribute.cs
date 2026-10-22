using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Toolbox_Library.Enums;
using Toolbox_Library.Repositories.Interfaces;
using Toolbox_Library.Services.Interfaces;
using Toolbox_Library.Utilities;
using Toolbox_Library.Utilities.Response;

namespace Toolbox_API.Extension
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public UserRole[] Roles { get; set; }

        public AuthorizeAttribute(params UserRole[] roles)
        {
            Roles = roles;
        }

        /// <summary>
        /// Authenticates the user that has called an endpoints and extracts the session key from the cookie to validate up against the redis database.
        /// Also authorizes the user after the authentication if they have the right role to perform the action.
        /// </summary>
        /// <param name="context"></param>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Try to get the session key from the cookies if the user has that cookie.
            string sessionKey = "";
            context.HttpContext.Request.Cookies.TryGetValue("toolbox-session", out string? sessionKeyFromCookie);
            if (!string.IsNullOrEmpty(sessionKeyFromCookie))
            {
                try
                {
                    // Make a Repository instance.
                    ISessionService sessionService = context.HttpContext.RequestServices.GetRequiredService<ISessionService>();

                    // Try to extract the data from the Redis database that has the key.
                    sessionKey = sessionKeyFromCookie;
                    SessionData sessionData = await sessionService.GetSessionDataFromKey(sessionKey);

                    // Check if the if the Role is null or empty which means any role is allowed.
                    // If both conditions are false then check the specific role matches the role extracted from the session key
                    if (Roles == null || Roles.Length == 0 || Roles.Contains(sessionData.Role))
                    {
                        // Stores data in a temporary server side dictionary.
                        context.HttpContext.Items["SessionData"] = sessionData; // Is deleted when API request is done.
                        return;
                    }
                }
                catch (HttpResponseException hre)
                {
                    // If a HttpResponseException was thrown something went wrong during the extraction. Convert the HttpResponse to a ObjectResult.
                    context.Result = new ObjectResult(new { message = hre.ResponseMessage })
                    {
                        StatusCode = (int)hre.StatusCode
                    };
                    return;
                }
            }
            context.Result = new UnauthorizedResult();
            return;
        }
    }
}
