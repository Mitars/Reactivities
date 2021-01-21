using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Reactivities.Application.Errors;
using Reactivities.Application.Interfaces;
using Reactivities.Persistence;

namespace Reactivities.Application.Photos
{
    public static class SetMain
    {
        public record Command : IRequest<Unit>
        {
            public string Id { get; init; }
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
                var user = this.context.Users.FirstOrDefault(user => user.UserName == this.userAccessor.GetCurrentUserName());
                var photo = user.Photos.FirstOrDefault(p => p.Id == request.Id);
                if (photo == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Photo = "Not found" });
                }
                else if (photo.IsMain)
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { Photo = "You cannot delete your main photo" });
                }

                var mainPhoto = user.Photos.FirstOrDefault(p => p.IsMain);
                mainPhoto.IsMain = false;
                photo.IsMain = true;

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