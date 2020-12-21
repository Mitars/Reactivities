using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Reactivities.Application.Profiles
{
    public class Details
    {
        public record Query : IRequest<Profile>
        {
            public string Username { get; init; }
        }
        
        public class Handler : IRequestHandler<Query, Profile>
        {
            private readonly IProfileReader profileReader;

            public Handler(IProfileReader profileReader)
            {
                this.profileReader = profileReader;
            }
            public async Task<Profile> Handle(Query request, CancellationToken cancellationToken) =>
                await this.profileReader.ReadProfile(request.Username);
        }
    }
}