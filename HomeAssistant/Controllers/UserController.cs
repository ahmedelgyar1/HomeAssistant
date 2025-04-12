using HomeAssistant.DTOs;
using HomeAssistant.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeAssistant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password,
                Address = dto.Address,
                AdminRole = dto.AdminRole
            };

            var success = await _userService.RegisterUserAsync(user);
            if (!success)
                return BadRequest("Email already exists.");

            return Ok("User registered successfully.");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userService.LoginAsync(loginDto.Email, loginDto.Password);
            if (user == null)
                return Unauthorized("Invalid email or password.");

            return Ok(new
            {
                user.Id,
                user.Name,
                user.Email,
                user.Address,
                user.AdminRole
            });
        }
    }
}
