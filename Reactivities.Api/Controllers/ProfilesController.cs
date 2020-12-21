using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reactivities.Application.Profiles;

namespace Reactivities.Api.Controllers
{
    public class ProfilesController : MediatorControllerBase
    {
        [HttpGet("{username}")]
        public async Task<ActionResult<Profile>> Get(string username) =>
            await Mediator.Send(new Details.Query { Username = username });

        [HttpGet("{username}/activities")]
        public async Task<ActionResult<List<UserActivityDto>>> GetUserActivities(string username, string predicate) =>
            await Mediator.Send(new ListActivities.Query { Username = username, Predicate = predicate });

        [HttpPut]
        public async Task<ActionResult<Unit>> Edit(Edit.Command profile) =>
            await Mediator.Send(profile);
    }
}