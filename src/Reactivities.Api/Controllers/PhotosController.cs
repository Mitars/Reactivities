using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reactivities.Application.Photos;
using Reactivities.Domain;

namespace Reactivities.Api.Controllers
{
    public class PhotosController : MediatorControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Photo>> Add([FromForm] Add.Command command) =>
            await Mediator.Send(command);

        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Delete(string id) =>
            await Mediator.Send(new Delete.Command { Id = id });

        [HttpPost("{id}/setmain")]
        public async Task<ActionResult<Unit>> SetMain(string id) =>
            await Mediator.Send(new SetMain.Command { Id = id });
    }
}