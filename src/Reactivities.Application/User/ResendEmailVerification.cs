using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Reactivities.Application.Errors;
using Reactivities.Application.Interfaces;
using Reactivities.Domain;

namespace Reactivities.Application.User
{
    public static class ResendEmailVerification
    {
        public record Query : IRequest
        {
            public string Email { get; init; }
            public string Origin { get; init; }
        }

        public class Handler : IRequestHandler<Query>
        {
            private readonly UserManager<AppUser> userManager;
            private readonly IEmailSender emailSender;

            public Handler(UserManager<AppUser> userManager, IEmailSender emailSender)
            {
                this.userManager = userManager;
                this.emailSender = emailSender;
            }

            public async Task<Unit> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await this.userManager.FindByEmailAsync(request.Email);
                if (user.EmailConfirmed)
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already verified. Please login" });
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