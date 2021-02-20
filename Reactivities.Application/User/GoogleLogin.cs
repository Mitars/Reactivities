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
    public static class GoogleLogin
    {
        public record Query : IRequest<UserDto>
        {
            public string AccessToken { get; init; }
        }

        public class Handler : IRequestHandler<Query, UserDto>
        {
            private readonly UserManager<AppUser> userManager;
            private readonly IGoogleAccessor googleAccessor;
            private readonly IJwtGenerator jwtGenerator;

            public Handler(UserManager<AppUser> userManager, IGoogleAccessor googleAccessor, IJwtGenerator jwtGenerator)
            {
                this.jwtGenerator = jwtGenerator;
                this.googleAccessor = googleAccessor;
                this.userManager = userManager;
            }

            public async Task<UserDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var userInfo = await this.googleAccessor.Login(request.AccessToken);
                if (userInfo == null)
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { User = "Problem validating token" });
                }

                var user = await this.userManager.FindByEmailAsync(userInfo.Email);
                if (user == null)
                {
                    user = new AppUser
                    {
                        DisplayName = userInfo.Name,
                        Id = userInfo.Id,
                        Email = userInfo.Email,
                        UserName = userInfo.Username,
                        EmailConfirmed = true
                    };

                    var photo = new Photo
                    {
                        Id = userInfo.Username,
                        Url = userInfo.PictureUrl,
                        IsMain = true
                    };

                    user.Photos.Add(photo);

                    var result = await this.userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        throw new RestException(HttpStatusCode.BadRequest, new { User = "Problem creating user" });
                    }
                }

                var refreshToken = this.jwtGenerator.GenerateRefreshToken();
                user.RefreshTokens.Add(refreshToken);
                await this.userManager.UpdateAsync(user);

                return new UserDto(user, jwtGenerator, refreshToken.Token);
            }
        }
    }
}