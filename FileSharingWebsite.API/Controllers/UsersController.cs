using FileSharingWebsite.Entities.Models;
using FileSharingWebsite.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileSharingWebsite.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("id/{id}")]
        public IActionResult GetUser(int id)
        {
            User user = userService.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("email/{email}")]
        public IActionResult GetUserByEmail(string email)
        {
            User user = userService.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] User newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            userService.CreateUser(newUser);

            return CreatedAtAction(nameof(CreateUser), new { success = true });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User updateUser)
        {

            var existingUser = userService.GetUser(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.Email = updateUser.Email;
            existingUser.Password = updateUser.Password;

            userService.UpdateUser(existingUser);

            return Ok(new { success = true });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            userService.DeleteUser(id);
            return Ok();
        }
    }
}
