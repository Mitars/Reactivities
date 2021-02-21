using System;
using System.Net;

namespace Reactivities.Application.Errors
{
    public class RestException : Exception
    {
        public RestException(HttpStatusCode code, object Errors = null)
        {
            this.Code = code;
            this.Errors = Errors;
        }

        public RestException()
        {
            this.Errors = HttpStatusCode.InternalServerError;
        }

        public RestException(string message) : base(message)
        {
            this.Errors = HttpStatusCode.InternalServerError;
        }

        public RestException(string message, Exception innerException) : base(message, innerException)
        {
            this.Errors = HttpStatusCode.InternalServerError;
        }

        public HttpStatusCode Code { get; }
        public object Errors { get; }
    }
}