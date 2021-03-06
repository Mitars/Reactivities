using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Errors;
using Reactivities.Domain;
using Reactivities.Persistence;
using Reactivities.Persistence.Helpers;

namespace Reactivities.Application.Comments
{
    public static class Create
    {
        public record Command : IRequest<CommentDto>
        {
            public string Body { get; init; }
            public Guid ActivityId { get; init; }
            public string Username { get; init; }
        }

        public class Handler : IRequestHandler<Command, CommentDto>
        {
            private readonly DataContext context;
            private readonly IMapper mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }

            public async Task<CommentDto> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await context.Activities.FindByIdAsync(request.ActivityId, cancellationToken);
                if (activity == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Activity = "Not Found" });
                }

                var user = await this.context.Users.SingleOrDefaultAsync(x => x.UserName == request.Username, cancellationToken);
                var comment = new Comment
                {
                    Author = user,
                    Activity = activity,
                    Body = request.Body,
                    CreatedAt = DateTime.Now
                };

                activity.Comments.Add(comment);

                var success = await this.context.SaveChangesAsync(cancellationToken) > 0;
                if (!success)
                {
                    throw new Exception("Problem saving changes");
                }

                return this.mapper.Map<CommentDto>(comment);
            }
        }
    }
}