//using Microsoft.AspNetCore.Mvc;
//using HomeAssistant.DTOs;
//using HomeAssistant.Builders; 
//namespace HomeAssistant.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class RoomController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public RoomController(AppDbContext context)
//        {
            
//            _context = context;
//        }

//        [HttpPost("Add Room")]
//        public async Task<IActionResult> AddRoom([FromBody] RoomDto roomDto)
//        {
//            var roomBuilder = new RoomBuilder(_context);

//            roomBuilder.WithName(roomDto.Name);

//            if (roomDto.Devices != null)
//            {
//                foreach (var deviceDto in roomDto.Devices)
//                {
//                    var device = new Device
//                    {
//                        Name = deviceDto.Name,
//                        Type = deviceDto.Type,
//                        Status = deviceDto.Status
//                    };

//                    roomBuilder.AddDevice(device);
//                }
//            }

//            if (roomDto.Cameras != null)
//            {
//                foreach (var cameraDto in roomDto.Cameras)
//                {
//                    var camera = new CameraStream
//                    {
//                        Name = cameraDto.Name,
//                        Url = cameraDto.Url
//                    };

//                    roomBuilder.AddCamera(camera);
//                }
//            }

//            var room = await roomBuilder.BuildAndSaveAsync();
//            return Ok(room);
//        }
//    }

//}
