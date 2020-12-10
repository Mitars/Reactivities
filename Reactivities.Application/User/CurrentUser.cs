using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Reactivities.Application.Interfaces;
using Reactivities.Domain;

namespace Reactivities.Application.User
{
    public class CurrentUser
    {
        public record Query : IRequest<Response> { }

        public record Response
        {
            public string DisplayName { get; init; }
            public string Token { get; init; }
            public string UserName { get; init; }
            public string Image { get; init; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly UserManager<AppUser> userManager;
            private readonly IJwtGenerator jwtGenerator;
            private readonly IUserAccessor userAccessor;

            public Handler(UserManager<AppUser> userManager, IJwtGenerator jwtGenerator, IUserAccessor userAccessor)
            {
                this.userAccessor = userAccessor;
                this.jwtGenerator = jwtGenerator;
                this.userManager = userManager;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await this.userManager.FindByNameAsync(this.userAccessor.GetCurrentUserName());

                return new Response
                {
                    DisplayName = user.DisplayName,
                    UserName = user.UserName,
                    Token = jwtGenerator.CreateToken(user),
                    Image = user.Photos.FirstOrDefault(photo => photo.IsMain)?.Url
                };
            }
        }
    }
}
