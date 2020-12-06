using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Reactivities.Application.Interfaces;

namespace Reactivities.Infrastructure.Security
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUserName() =>
            this.httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}