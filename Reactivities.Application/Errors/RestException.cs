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

        public HttpStatusCode Code { get; }
        public object Errors { get; }
    }
}