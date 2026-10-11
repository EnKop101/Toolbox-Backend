using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox_Library.Utilities.Response
{
    /// Represents categorized errors mapped to HTTP status codes.
    public enum ServiceReturnCode
    {
        // Request was a success but no data was found.
        NoContent = 204,

        // Invalid request data.
        BadRequest = 400,

        // Authorization / Authentication failed.
        Unauthorized = 401,

        // Access to the resource is forbidden.
        Forbidden = 403,

        // The requested resource was not found.
        NotFound = 404,

        // A conflict occurred with the current system state.
        Conflict = 409,

        // An unexpected internal server error occurred.
        InternalError = 500
    }
}
