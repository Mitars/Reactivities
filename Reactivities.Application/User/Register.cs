using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Errors;
using Reactivities.Application.Interfaces;
using Reactivities.Application.Validators;
using Reactivities.Domain;
using Reactivities.Persistence;

namespace Reactivities.Application.User
{
    public static class Register
    {
        public record Command : IRequest
        {
            public string DisplayName { get; init; }
            public string UserName { get; init; }
            public string Email { get; init; }
            public string Password { get; init; }
            public string Origin { get; init; }
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

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext context;
            private readonly UserManager<AppUser> userManager;
            private readonly IEmailSender emailSender;

            public Handler(DataContext context, UserManager<AppUser> userManager, IEmailSender emailSender)
            {
                this.emailSender = emailSender;
                this.context = context;
                this.userManager = userManager;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                if (await this.context.Users.AnyAsync(user => user.Email == request.Email, cancellationToken))
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already exists" });
                }

                if (await this.context.Users.AnyAsync(user => user.UserName == request.UserName, cancellationToken))
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

                var token = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var verifyUrl = $"{request.Origin}/user/verifyEmail?token={token}&email={request.Email}";

                var message = $"<p>Please click the below link to verify your email address:</p><p><a href='{verifyUrl}'>{verifyUrl}></a></p>";
                await this.emailSender.SendEmailAsync(request.Email, "Please verify the email address", message);

                return Unit.Value;
            }
        }
    }
}