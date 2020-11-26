using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Reactivities.Persistence;

namespace Reactivities.Application.Activities
{
    public class Delete
    {
        public record Command : IRequest
        {
            public Guid Id { get; init; }
        }
        
        public record Handler : IRequestHandler<Command>
        {
            private readonly DataContext context;
        
            public Handler(DataContext context) {
                this.context = context;
            }
        
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await this.context.Activities.FindAsync(request.Id);

                if (activity == null) {
                    throw new Exception($"Could not find activity with ID {request.Id}");
                }

                this.context.Activities.Remove(activity);                
                var success = await this.context.SaveChangesAsync() > 0;
        
                if (success) return Unit.Value;
        
                throw new Exception("Problem saving changes");
            }
        }
    }
}