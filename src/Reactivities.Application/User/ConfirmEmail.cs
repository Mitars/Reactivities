using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Reactivities.Domain;

namespace Reactivities.Application.User
{
    public static class ConfirmEmail
    {
        public record Command : IRequest<IdentityResult>
        {
            public string Token { get; init; }
            public string Email { get; init; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Token).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, IdentityResult>
        {
            private readonly UserManager<AppUser> userManager;
            public Handler(UserManager<AppUser> userManager)
            {
                this.userManager = userManager;
            }

            public async Task<IdentityResult> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await this.userManager.FindByEmailAsync(request.Email);
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(request.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);
                return await this.userManager.ConfirmEmailAsync(user, decodedToken);
            }
        }
    }
}