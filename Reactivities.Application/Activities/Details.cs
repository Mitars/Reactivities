using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Reactivities.Application.Errors;
using Reactivities.Domain;
using Reactivities.Persistence;
using Reactivities.Persistence.Helpers;

namespace Reactivities.Application.Activities
{
    public static class Details
    {
        public record Query : IRequest<ActivityDto>
        {
            public Guid Id { get; init; }
        }

        public class Handler : IRequestHandler<Query, ActivityDto>
        {
            private readonly DataContext context;
            private readonly IMapper mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }

            public async Task<ActivityDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var activity = await this.context.Activities.FindByIdAsync(request.Id, cancellationToken);

                if (activity == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { activity = "Not found" });
                }

                return this.mapper.Map<Activity, ActivityDto>(activity);
            }
        }
    }
}
