using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Reactivities.Application.Errors;
using Reactivities.Application.Interfaces;
using Reactivities.Domain;

namespace Reactivities.Application.User
{
    public class RefreshToken
    {
        public record Command : IRequest<UserDto>
        {
            public string RefreshToken { get; init; }
        }

        public class Handler : IRequestHandler<Command, UserDto>
        {
            private readonly UserManager<AppUser> userManager;
            private readonly IJwtGenerator jwtGenerator;
            private readonly IUserAccessor userAccessor;
            public Handler(UserManager<AppUser> userManager, IJwtGenerator jwtGenerator, IUserAccessor userAccessor)
            {
                this.userManager = userManager;
                this.jwtGenerator = jwtGenerator;
                this.userAccessor = userAccessor;
            }

            public async Task<UserDto> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await this.userManager.FindByNameAsync(this.userAccessor.GetCurrentUserName());
                var oldToken = user.RefreshTokens.SingleOrDefault(t => t.Token == request.RefreshToken);

                if (oldToken != null && !oldToken.IsActive)
                {
                    throw new RestException(HttpStatusCode.Unauthorized);
                }

                if (oldToken != null) 
                {
                    oldToken.Revoked = DateTime.UtcNow;
                }

                var newRefreshToken = this.jwtGenerator.GenerateRefreshToken();
                user.RefreshTokens.Add(newRefreshToken);

                await this.userManager.UpdateAsync(user);

                return new UserDto(user, this.jwtGenerator, newRefreshToken.Token);
            }
        }
    }
}