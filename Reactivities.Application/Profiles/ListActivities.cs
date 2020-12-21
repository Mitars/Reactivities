using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Reactivities.Application.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reactivities.Persistence;
using Reactivities.Domain;

namespace Reactivities.Application.Profiles
{
    public class ListActivities
    {
        public record Query : IRequest<List<UserActivityDto>>
        {
            public string Username { get; init; }
            public string Predicate { get; init; }
        }

        public class Handler : IRequestHandler<Query, List<UserActivityDto>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<List<UserActivityDto>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == request.Username);

                if (user == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { User = "Not found" });
                }

                Func<UserActivity, bool> filterAction = request.Predicate switch
                {
                    "past" =>
                        (UserActivity a) => a.Activity.Date <= DateTime.Now,
                    "hosting" =>
                        (UserActivity a) => a.IsHost,
                    _ =>
                        (UserActivity a) => a.Activity.Date >= DateTime.Now,
                };

                return user.UserActivities
                    .OrderBy(a => a.Activity.Date).AsQueryable()
                    .Where(filterAction)
                    .ToList()
                    .Select(activity => new UserActivityDto
                    {
                        Id = activity.Activity.Id,
                        Title = activity.Activity.Title,
                        Category = activity.Activity.Category,
                        Date = activity.Activity.Date
                    }).ToList();
            }
        }
    }
}