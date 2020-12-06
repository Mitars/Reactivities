using Microsoft.AspNetCore.Identity;

namespace Reactivities.Domain
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
    }
}