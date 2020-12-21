using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Errors;
using Reactivities.Application.Interfaces;
using Reactivities.Persistence;

namespace Reactivities.Application.Followers
{
    public class Delete
    {
        public record Command : IRequest
        {
            public string Username { get; init; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext context;
            private readonly IUserAccessor userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                this.context = context;
                this.userAccessor = userAccessor;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var currentUser = await this.context.Users
                    .Include(u => u.Followings)
                    .Include(u => u.Followers)
                    .SingleOrDefaultAsync(u => u.UserName == this.userAccessor.GetCurrentUserName());
                var targetUser = await this.context.Users.SingleOrDefaultAsync(u => u.UserName == request.Username);

                if (targetUser == null) {
                    throw new RestException(HttpStatusCode.NotFound, new { User = "Not found" });
                }

                if (currentUser.Followings.All(u => u.Id != targetUser.Id)) {
                    throw new RestException(HttpStatusCode.BadRequest, new { User = "You are not following this user" });
                }

                currentUser.Followings.Remove(targetUser);

                var success = await this.context.SaveChangesAsync() > 0;

                if (success) return Unit.Value;

                throw new Exception("Problem saving changes");
            }
        }
    }
}