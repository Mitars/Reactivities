using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Reactivities.Domain
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public virtual ICollection<UserActivity> UserActivities { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
        public virtual ICollection<AppUser> Followings { get; set; }
        public virtual ICollection<AppUser> Followers { get; set; }
    }
}