using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Reactivities.Application.Profiles;

namespace Reactivities.Api.Controllers
{
    public class ProfilesController : MediatorControllerBase
    {
        [HttpGet("{username}")]
        public async Task<ActionResult<Profile>> Get(string username) =>
            await Mediator.Send(new Details.Query { Username = username });
    }
}