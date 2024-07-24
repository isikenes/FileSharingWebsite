using FileSharingWebsite.Entities.Models;
using FileSharingWebsite.Helpers;
using FileSharingWebsite.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileSharingWebsite.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;

        public AuthController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = userService.GetUserByEmail(loginRequest.Email);

            if (user != null && PasswordHelper.VerifyPassword(loginRequest.Password, user.Password))
            {
                return Ok(new LoginResponse { IsSuccess = true, Message = "Login successful", UserId = user.UserId });
            }

            return Unauthorized(new LoginResponse { IsSuccess = false, Message = "Invalid login attempt", UserId = -1 });
        }
    }
}
