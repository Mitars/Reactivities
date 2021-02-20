using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Identity;

namespace Reactivities.Domain
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            Photos = new Collection<Photo>();
            RefreshTokens = new Collection<RefreshToken>();
        }

        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public virtual ICollection<UserActivity> UserActivities { get; init; }
        public virtual ICollection<Photo> Photos { get; init; }
        public virtual ICollection<AppUser> Followings { get; init; }
        public virtual ICollection<AppUser> Followers { get; init; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; init; }
    }
}
