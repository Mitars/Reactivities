using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Reactivities.Domain;
using Reactivities.Persistence;

namespace Reactivities.Application.Activities
{
    public class Details
    {
        public record Query : IRequest<Activity>
        {
            public Guid Id { get; init; }
        }

        public class Handler : IRequestHandler<Query, Activity>
        {
            private readonly DataContext context;

            public Handler(DataContext context)
            {
                this.context = context;
            }

            public async Task<Activity> Handle(Query request, CancellationToken cancellationToken) =>
                await this.context.Activities.FindAsync(request.Id);
        }
    }
}