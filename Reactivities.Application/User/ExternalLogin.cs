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
        public record Query : IRequest<Response> {
            public string AccessToken { get; init; }
        }

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
            private readonly IFacebookAccessor facebookAccessor;
            private readonly IJwtGenerator jwtGenerator;

            public Handler(UserManager<AppUser> userManager, IFacebookAccessor facebookAccessor, IJwtGenerator jwtGenerator)
            {
                this.jwtGenerator = jwtGenerator;
                this.facebookAccessor = facebookAccessor;
                this.userManager = userManager;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
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

                return new Response
                {
                    DisplayName = user.DisplayName,
                    Token = this.jwtGenerator.CreateToken(user),
                    UserName = user.UserName,
                    Image = user.Photos.FirstOrDefault(p => p.IsMain)?.Url
                };
            }
        }
    }
}