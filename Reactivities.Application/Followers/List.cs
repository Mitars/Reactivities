using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Errors;
using Reactivities.Application.Profiles;
using Reactivities.Persistence;

namespace Reactivities.Application.Followers
{
    public static class List
    {
        public record Query : IRequest<List<Profile>>
        {
            public string Username { get; init; }
            public string Predicate { get; init; }
        }

        public class Handler : IRequestHandler<Query, List<Profile>>
        {
            private readonly DataContext context;
            private readonly IProfileReader profileReader;

            public Handler(DataContext context, IProfileReader profileReader)
            {
                this.context = context;
                this.profileReader = profileReader;
            }

            public async Task<List<Profile>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await this.context.Users.SingleOrDefaultAsync(u => u.UserName == request.Username, cancellationToken);
                return request.Predicate switch
                {
                    "following" => (await Task.WhenAll(user.Followings.Select(async u => await this.profileReader.ReadProfile(u.UserName)))).ToList(),
                    "followers" => (await Task.WhenAll(user.Followers.Select(async u => await this.profileReader.ReadProfile(u.UserName)))).ToList(),
                    _ => throw new RestException(HttpStatusCode.BadRequest, new { Predicate = "Must be either following or followers" }),
                };
            }
        }
    }
}