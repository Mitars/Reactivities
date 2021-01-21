using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Reactivities.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediatorControllerBase : ControllerBase
    {
        private IMediator mediator;

        protected IMediator Mediator =>
            this.mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    }
}