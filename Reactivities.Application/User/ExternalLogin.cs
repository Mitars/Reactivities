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
    public class ExternalLogin
    {
        public record Query : IRequest<UserDto> {
            public string AccessToken { get; init; }
        }

        public class Handler : IRequestHandler<Query, UserDto>
        {
            private readonly UserManager<AppUser> userManager;
            private readonly IFacebookAccessor facebookAccessor;
            private readonly IJwtGenerator jwtGenerator;

            public Handler(UserManager<AppUser> userManager, IFacebookAccessor facebookAccessor, IJwtGenerator jwtGenerator)
            {
                this.jwtGenerator = jwtGenerator;
                this.facebookAccessor = facebookAccessor;
                this.userManager = userManager;
            }

            public async Task<UserDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var userInfo = await this.facebookAccessor.FacebookLogin(request.AccessToken);

                if (userInfo == null)
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { User = "Problem validating token"});
                }

                var user = await this.userManager.FindByEmailAsync(userInfo.Email);

                if (user == null)
                {
                    user = new AppUser
                    {
                        DisplayName = userInfo.Name,
                        Id = userInfo.Id,
                        Email = userInfo.Email,
                        UserName = "fb_" + userInfo.Id,
                    };

                    var photo = new Photo
                    {
                        Id = "fb_" + userInfo.Id,
                        Url = userInfo.Picture.Data.Url,
                        IsMain = true
                    };

                    user.Photos.Add(photo);

                    var result = await this.userManager.CreateAsync(user);

                    if (!result.Succeeded)
                    {
                        throw new RestException(HttpStatusCode.BadRequest, new { User = "Problem creating user"});
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