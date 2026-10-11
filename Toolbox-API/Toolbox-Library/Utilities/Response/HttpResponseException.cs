using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox_Library.Utilities.Response
{
    public class HttpResponseException : Exception
    {
        public ServiceReturnCode StatusCode { get; }
        public string ResponseMessage { get; }

        // Polymorphed class that is used for the Try Catch to handle all the different errors.
        public HttpResponseException(ServiceReturnCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
            ResponseMessage = message;
        }

        // This is the same as making a constructor and a base() in one
        public class BadRequestException(string message) : HttpResponseException(ServiceReturnCode.BadRequest, message) { }

        public class NotFoundException(string message) : HttpResponseException(ServiceReturnCode.NotFound, message) { }

        public class InternalErrorException(string message) : HttpResponseException(ServiceReturnCode.InternalError, message) { }

        public class UnauthorizedException(string message) : HttpResponseException(ServiceReturnCode.Unauthorized, message) { }

        public class Forbidden(string message) : HttpResponseException(ServiceReturnCode.Forbidden, message) { }

        public class NoContentException(string message) : HttpResponseException(ServiceReturnCode.NoContent, message) { }

        public class ConflictException(string message) : HttpResponseException(ServiceReturnCode.Conflict, message) { }
    }
}
