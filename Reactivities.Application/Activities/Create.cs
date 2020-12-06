using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Reactivities.Domain;
using Reactivities.Persistence;

namespace Reactivities.Application.Activities
{
    public class Create
    {
        public record Command : IRequest
        {
            public Guid Id { get; init; }
            public string Title { get; init; }
            public string Description { get; init; }
            public string Category { get; init; }
            public DateTime Date { get; init; }
            public string City { get; init; }
            public string Venue { get; init; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.Title).NotEmpty();
                RuleFor(c => c.Description).NotEmpty();
                RuleFor(c => c.Category).NotEmpty();
                RuleFor(c => c.Date).NotEmpty();
                RuleFor(c => c.City).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext context;

            public Handler(DataContext context)
            {
                this.context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = new Activity
                {
                    Id = request.Id,
                    Title = request.Title,
                    Description = request.Description,
                    Category = request.Category,
                    Date = request.Date,
                    City = request.City,
                    Venue = request.Venue,
                };

                this.context.Activities.Add(activity);
                var success = await this.context.SaveChangesAsync() > 0;

                if (success) return Unit.Value;

                throw new Exception("Problem saving changes");
            }
        }
    }
}