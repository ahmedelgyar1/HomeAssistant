using HomeAssistant.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HomeAssistant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly RoomService _roomService;

        public RoomController(RoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddRoom([FromBody] string name)
        {
            var room = await _roomService.AddRoomAsync(name);
            return Ok(new { message = "Room added successfully", room });
        }

        [HttpDelete("delete/{Name}")]
        public async Task<IActionResult> DeleteRoom(string Name)
        {
            var deleted = await _roomService.DeleteRoomAsync(Name);
            if (!deleted)
                return NotFound($"Room with Name {Name} not found.");
            return Ok(new { message = "Room deleted successfully" });
        }
    }
}
