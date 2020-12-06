using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reactivities.Application.User;
using System.Threading.Tasks;

namespace Reactivities.Api.Controllers
{
    public class UserController : MediatorControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<CurrentUser.Response>> CurrentUser() =>
            await this.Mediator.Send(new CurrentUser.Query());

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<Login.Response>> Login(Login.Query command) =>
            await this.Mediator.Send(command);

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<Register.Response>> Register(Register.Command command) =>
            await this.Mediator.Send(command);
    }
}