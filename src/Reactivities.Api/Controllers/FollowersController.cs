using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reactivities.Application.Followers;
using Reactivities.Application.Profiles;

namespace Reactivities.Api.Controllers
{
    [Route("api/profiles")]
    public class FollowersController : MediatorControllerBase
    {
        [HttpGet("{username}/follow")]
        public async Task<ActionResult<List<Profile>>> GetFollowings(string username, string predicate) =>
            await Mediator.Send(new List.Query { Username = username, Predicate = predicate });

        [HttpPost("{username}/follow")]
        public async Task<ActionResult<Unit>> Follow(string username) =>
            await Mediator.Send(new Add.Command { Username = username });

        [HttpDelete("{username}/follow")]
        public async Task<ActionResult<Unit>> Unfollow(string username) =>
            await Mediator.Send(new Delete.Command { Username = username });
    }
}