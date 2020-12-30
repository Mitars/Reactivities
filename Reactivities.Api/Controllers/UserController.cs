using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reactivities.Application.User;
using System;
using System.Threading.Tasks;

namespace Reactivities.Api.Controllers
{
    public class UserController : MediatorControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<UserDto>> CurrentUser() =>
            await this.Mediator.Send(new CurrentUser.Query());

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(Login.Query command) =>
            this.AppendUserRefreshTokenCookie(await this.Mediator.Send(command));

        [AllowAnonymous]
        [HttpPost("facebook")]
        public async Task<ActionResult<UserDto>> FacebookLogin(ExternalLogin.Query command) =>
            this.AppendUserRefreshTokenCookie(await this.Mediator.Send(command));

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(Register.Command command) =>
            this.AppendUserRefreshTokenCookie(await this.Mediator.Send(command));
        
        [HttpPost("refreshToken")]
        public async Task<ActionResult<UserDto>> RefreshToken(RefreshToken.Command command) =>
            AppendUserRefreshTokenCookie(await this.Mediator.Send(command with { RefreshToken = Request.Cookies["refreshToken"] }));

        private UserDto AppendUserRefreshTokenCookie(UserDto user)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", user.RefreshToken, cookieOptions);
            return user;
        }
    }
}