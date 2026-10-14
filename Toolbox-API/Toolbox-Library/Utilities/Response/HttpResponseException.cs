using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Toolbox_Library.Utilities.Response
{
    public class HttpResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string ResponseMessage { get; }

        // Polymorphed class that is used for the Try Catch to handle all the different errors.
        public HttpResponseException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
            ResponseMessage = message;
        }

        // This is the same as making a constructor and a base() in one
        public class BadRequestException(string message) : HttpResponseException(HttpStatusCode.BadRequest, message) { }

        public class NotFoundException(string message) : HttpResponseException(HttpStatusCode.NotFound, message) { }

        public class InternalErrorException(string message) : HttpResponseException(HttpStatusCode.InternalServerError, message) { }

        public class UnauthorizedException(string message) : HttpResponseException(HttpStatusCode.Unauthorized, message) { }

        public class Forbidden(string message) : HttpResponseException(HttpStatusCode.Forbidden, message) { }

        public class NoContentException(string message) : HttpResponseException(HttpStatusCode.NoContent, message) { }

        public class ConflictException(string message) : HttpResponseException(HttpStatusCode.Conflict, message) { }
    }
}
