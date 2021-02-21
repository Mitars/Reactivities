using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Interfaces;
using Reactivities.Domain;
using Reactivities.Persistence;

namespace Reactivities.Application.Activities
{
    public static class List
    {
        public record Query : IRequest<Response>
        {
            public int? Limit { get; init; }
            public int? Offset { get; init; }
            public bool IsGoing { get; init; }
            public bool IsHost { get; init; }
            public DateTime? StartDate { get; init; } = DateTime.Now;
        }

        public record Response
        {
            public List<ActivityDto> Activities { get; init; }
            public int ActivityCount { get; init; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly DataContext context;
            private readonly IMapper mapper;
            private readonly IUserAccessor userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                this.context = context;
                this.mapper = mapper;
                this.userAccessor = userAccessor;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var currentUserName = userAccessor.GetCurrentUserName();
                var activities = this.context.Activities
                    .Where(a => a.Date >= request.StartDate)
                    .OrderBy(a => a.Date)
                    .Where(a => !request.IsGoing || a.UserActivities.Any(u => u.AppUser.UserName == currentUserName))
                    .Where(a => !request.IsHost || a.UserActivities.Any(u => u.AppUser.UserName == currentUserName && u.IsHost));

                var activitiesCount = await activities.CountAsync(cancellationToken);
                var activitiesFiltered = await activities.Skip(request.Offset ?? 0)
                    .Take(request.Limit ?? 3).ToListAsync(cancellationToken);

                return new Response
                {
                    Activities = this.mapper.Map<List<Activity>, List<ActivityDto>>(activitiesFiltered),
                    ActivityCount = activitiesCount,
                };
            }
        }
    }
}
