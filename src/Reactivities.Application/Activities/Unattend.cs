using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Errors;
using Reactivities.Application.Interfaces;
using Reactivities.Persistence;
using Reactivities.Persistence.Helpers;

namespace Reactivities.Application.Activities
{
    public static class Unattend
    {
        public record Command : IRequest
        {
            public Guid Id { get; init; }
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
                var activity = await this.context.Activities.FindByIdAsync(request.Id, cancellationToken);
                if (activity == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Activity = "Could not find activity" });
                }

                var user = await this.context.Users.SingleOrDefaultAsync(u => u.UserName == this.userAccessor.GetCurrentUserName(), cancellationToken);

                var attendance = await this.context.UserActivities.SingleOrDefaultAsync(ua => ua.ActivityId == activity.Id && ua.AppUserId == user.Id, cancellationToken);
                if (attendance == null)
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { Attendance = "Not attending this activity" });
                }
                else if (attendance.IsHost)
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { Attendance = "You cannot remove yourself as host" });
                }

                this.context.UserActivities.Remove(attendance);

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