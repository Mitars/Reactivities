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
                .Append("style-src 'self' https://fonts.googleapis.com/ 'sha256-wkAU1AW/h8YFx0XlzvpTllAKnFEO2tw8aKErs5a26LY=';")
                .Append("font-src 'self' https://fonts.gstatic.com/ data:;")
                .Append("form-action 'self';")
                .Append("frame-ancestors 'self';")
                .Append("img-src 'self' https://res.cloudinary.com data: blob:;")
                .Append("script-src 'self' 'sha256-uSE6wZK79ac9dp/vledp4pXmIVW3QpEYGeScfy6MhnY=' 'sha256-rmPzuTgq1ESh5M2NbjxubD7q+sQYBgY660mouzJfn4Q=' 'sha256-Tui7QoFlnLXkJCSl1/JvEZdIXTmBttnWNxzJpXomQjg=' 'sha256-JRz1/3X5iBvaty8nuXJ+IMllcFH+any3UGyhGRZ3i9g=' 'sha256-jhSY8gx6xQqltOsXobsXomDNSPOpQSuFg9wvBUvY+DI=';")
                .ToString();

            context.Response.Headers["Content-Security-Policy"] = contentSecurityPolicy;
            context.Response.Headers["X-Frame-Options"] = "DENY";
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["Referrer-Policy"] = "same-origin";
            context.Response.Headers["Permissions-Policy"] = "geolocation=(self '')";

            await next(context);
        }
    }
}
