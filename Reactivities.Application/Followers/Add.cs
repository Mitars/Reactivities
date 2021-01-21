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
    public static class Add
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
                    .SingleOrDefaultAsync(u => u.UserName == this.userAccessor.GetCurrentUserName(), cancellationToken);
                var targetUser = await this.context.Users.SingleOrDefaultAsync(u => u.UserName == request.Username, cancellationToken);
                if (targetUser == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { User = "Not found" });
                }
                else if (currentUser.Followings.Any(u => u.Id == targetUser.Id))
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { User = "You are already following this user" });
                }

                currentUser.Followings.Add(targetUser);

                var success = await this.context.SaveChangesAsync(cancellationToken) > 0;
                if (!success)
                {
                    throw new Exception("Problem saving changes");
                }

                return Unit.Value;
            }
        }
    }
}