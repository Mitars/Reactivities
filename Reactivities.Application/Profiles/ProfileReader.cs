using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Errors;
using Reactivities.Application.Interfaces;
using Reactivities.Persistence;

namespace Reactivities.Application.Profiles
{
    public class ProfileReader : IProfileReader
    {
        private readonly DataContext context;
        private readonly IUserAccessor userAccessor;

        public ProfileReader(DataContext context, IUserAccessor userAccessor)
        {
            this.context = context;
            this.userAccessor = userAccessor;
        }

        public async Task<Profile> ReadProfile(string username)
        {
            var user = await this.context.Users.SingleOrDefaultAsync(u => u.UserName == username);

            if(user == null)
            {
                throw new RestException(HttpStatusCode.BadRequest, new { User = "Not found" });
            }

            var currentUser = await this.context.Users.SingleOrDefaultAsync(u => u.UserName == this.userAccessor.GetCurrentUserName());

            return new Profile
            {
                DisplayName = user.DisplayName,
                Username = user.UserName,
                Image = user.Photos.FirstOrDefault(photo => photo.IsMain)?.Url,
                Photos = user.Photos,
                Bio = user.Bio,
                FollowersCount = user.Followers.Count(),
                FollowingCount = user.Followings.Count(),
                IsFollowed = currentUser.Followings.Any(u => u.Id == user.Id)
            };
        }
    }
}