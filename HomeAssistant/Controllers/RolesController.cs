using global::HomeAssistant.Services;
using HomeAssistant.Models;
using Microsoft.AspNetCore.Mvc;
using HomeAssistant.DTOs;

namespace HomeAssistant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly UserService _userService;

        public RolesController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPut("assign")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            var user = await _userService.GetByEmailAsync(dto.Email);
            if (user == null)
                return NotFound("User not found.");

            user.Role = dto.Role;
            await _userService.UpdateAsync(user);

            return Ok(new { message = "Role assigned successfully." });
        }
    }
}


