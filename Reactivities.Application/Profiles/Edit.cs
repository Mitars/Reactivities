using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Interfaces;
using Reactivities.Persistence;

namespace Reactivities.Application.Profiles
{
    public static class Edit
    {
        public record Command : IRequest
        {
            public string DisplayName { get; init; }
            public string Bio { get; init; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(u => u.DisplayName).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext context;
            private readonly IUserAccessor userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                this.context = context;
                this.userAccessor = userAccessor;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await this.context.Users.FirstOrDefaultAsync(u => u.UserName == this.userAccessor.GetCurrentUserName(), cancellationToken);

                user.DisplayName = request.DisplayName;
                user.Bio = request.Bio ?? user.Bio;

                var success = await this.context.SaveChangesAsync(cancellationToken) > 0;
                if (!success)
                {
                    throw new Exception("Problem saving changes");
                }

                return Unit.Value;
            }
        }
    }
}