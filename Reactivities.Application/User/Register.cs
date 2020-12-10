using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Errors;
using Reactivities.Application.Interfaces;
using Reactivities.Application.Validators;
using Reactivities.Domain;
using Reactivities.Persistence;

namespace Reactivities.Application.User
{
    public class Register
    {
        public record Command : IRequest<Response>
        {
            public string DisplayName { get; init; }
            public string UserName { get; init; }
            public string Email { get; init; }
            public string Password { get; init; }
        }

        public class QueryValidator : AbstractValidator<Command>
        {
            public QueryValidator()
            {
                RuleFor(q => q.DisplayName).NotEmpty();
                RuleFor(q => q.UserName).NotEmpty();
                RuleFor(q => q.Email).NotEmpty().EmailAddress();
                RuleFor(q => q.Password).Password();
            }
        }

        public record Response
        {
            public string DisplayName { get; init; }
            public string Token { get; init; }
            public string UserName { get; init; }
            public string Image { get; init; }
        }

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly DataContext context;
            private readonly UserManager<AppUser> userManager;
            private readonly IJwtGenerator jwtGenerator;

            public Handler(DataContext context, UserManager<AppUser> userManager, IJwtGenerator jwtGenerator)
            {
                this.context = context;
                this.userManager = userManager;
                this.jwtGenerator = jwtGenerator;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                if (await this.context.Users.AnyAsync(user => user.Email == request.Email))
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already exists" });
                }

                if (await this.context.Users.AnyAsync(user => user.UserName == request.UserName))
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { UserName = "User name already exists" });
                }

                var user = new AppUser
                {
                    DisplayName = request.DisplayName,
                    Email = request.Email,
                    UserName = request.UserName
                };

                var result = await this.userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    throw new Exception("Problem creating user");
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
