using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reactivities.Application.User;

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
        public async Task<ActionResult<UserDto>> FacebookLogin(FacebookLogin.Query command) =>
            this.AppendUserRefreshTokenCookie(await this.Mediator.Send(command));

        [AllowAnonymous]
        [HttpPost("google")]
        public async Task<ActionResult<UserDto>> GoogleLogin(GoogleLogin.Query command) =>
            this.AppendUserRefreshTokenCookie(await this.Mediator.Send(command));

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<Unit>> Register(Register.Command command)
        {
            await this.Mediator.Send(command with { Origin = Request.Headers["origin"] });
            return Ok("Registration successful - please check your email");
        }

        [HttpPost("refreshToken")]
        public async Task<ActionResult<UserDto>> RefreshToken(RefreshToken.Command command) =>
            AppendUserRefreshTokenCookie(await this.Mediator.Send(command with { RefreshToken = Request.Cookies["refreshToken"] }));

        [AllowAnonymous]
        [HttpPost("verifyEmail")]
        public async Task<ActionResult> VerifyEmail(ConfirmEmail.Command command)
        {
            var result = await this.Mediator.Send(command);

            if (!result.Succeeded)
            {
                return BadRequest("Problem verifying email address");
            }

            return Ok("Email confirmed - you can now login");
        }

        [AllowAnonymous]
        [HttpGet("resendEmailVerification")]
        public async Task<ActionResult> ResendEmailVerification([FromQuery] ResendEmailVerification.Query query)
        {
            await this.Mediator.Send(query with { Origin = Request.Headers["origin"] });
            return Ok("Email verification link sent - please check email");
        }

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