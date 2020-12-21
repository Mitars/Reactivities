using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Interfaces;
using Reactivities.Domain;
using Reactivities.Persistence;

namespace Reactivities.Application.Activities
{
    public class FolowingResolver : IValueResolver<UserActivity, AttendeeDto, bool>
    {
        private readonly DataContext context;
        private readonly IUserAccessor userAccessor;

        public FolowingResolver(DataContext context, IUserAccessor userAccessor)
        {
            this.context = context;
            this.userAccessor = userAccessor;
        }
        public bool Resolve(UserActivity source, AttendeeDto destination, bool destMember, ResolutionContext context) =>
            this.context.Users
                .SingleOrDefaultAsync(u => u.UserName == this.userAccessor.GetCurrentUserName()).GetAwaiter().GetResult()
                .Followings.Any(u => u.Id == source.AppUserId);
    }
}