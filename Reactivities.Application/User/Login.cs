using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Reactivities.Application.Errors;
using Reactivities.Application.Interfaces;
using Reactivities.Domain;

namespace Reactivities.Application.User
{
    public class Login
    {
        public record Query : IRequest<Response>
        {
            public string Email { get; init; }
            public string Password { get; init; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(q => q.Email).NotEmpty();
                RuleFor(q => q.Password).NotEmpty();
            }
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
            private readonly SignInManager<AppUser> signInManager;
            private readonly IJwtGenerator jwtGenerator;

            public Handler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtGenerator jwtGenerator)
            {
                this.jwtGenerator = jwtGenerator;
                this.userManager = userManager;
                this.signInManager = signInManager;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await this.userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    throw new RestException(HttpStatusCode.Unauthorized);
                }

                var result = await this.signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (!result.Succeeded)
                {
                    throw new RestException(HttpStatusCode.Unauthorized);
                }

                return new Response
                {
                    DisplayName = user.DisplayName,
                    Token = this.jwtGenerator.CreateToken(user),
                    UserName = user.UserName,
                    Image = user.Photos.FirstOrDefault(photo => photo.IsMain)?.Url
                };
            }
        }
    }
}
