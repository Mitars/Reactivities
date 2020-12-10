using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reactivities.Persistence;

namespace Reactivities.Application.Profiles
{
    public class Details
    {
        public record Query : IRequest<Profile>
        {
            public string Username { get; set; }
        }
        
        public class Handler : IRequestHandler<Query, Profile>
        {
            private readonly DataContext context;

            public Handler(DataContext context)
            {
                this.context = context;
            }

            public async Task<Profile> Handle(Query request, CancellationToken cancellationToken) {
                var user = await this.context.Users.SingleOrDefaultAsync(user => user.UserName == request.Username);

                return new Profile
                {
                    DisplayName = user.DisplayName,
                    Username = user.UserName,
                    Image = user.Photos.FirstOrDefault(photo => photo.IsMain)?.Url,
                    Photos = user.Photos,
                    Bio = user.Bio
                };
            }
        }
    }
}