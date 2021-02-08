using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Reactivities.Api.Middleware
{
    public class ResponseHeaderMiddleware
    {
        private readonly RequestDelegate next;
        public ResponseHeaderMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var contentSecurityPolicy = new StringBuilder()
                .Append("block-all-mixed-content;")
                .Append("style-src 'self' https://fonts.googleapis.com/ 'sha256-2aahydUs+he2AO0g7YZuG67RGvfE9VXGbycVgIwMnBI=';")
                .Append("font-src 'self' https://fonts.gstatic.com/ data:;")
                .Append("frame-ancestors 'self';")
                .Append("img-src 'self' https://res.cloudinary.com/ https://www.facebook.com/ data: blob:;")
                .Append("script-src 'self' 'sha256-rmPzuTgq1ESh5M2NbjxubD7q+sQYBgY660mouzJfn4Q=' https://connect.facebook.net/en_US/sdk.js;")
                .ToString();

            context.Response.Headers["Content-Security-Policy"] = contentSecurityPolicy;
            context.Response.Headers["X-Frame-Options"] = "DENY";
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
            context.Response.Headers["Referrer-Policy"] = "same-origin";
            context.Response.Headers["Permissions-Policy"] = "geolocation=()";

            await next(context);
        }
    }
}
