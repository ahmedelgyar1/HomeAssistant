using HomeAssistant.DTOs;
using HomeAssistant.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HomeAssistant.Services
{
    public class DeviceService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public DeviceService(
            IHttpClientFactory httpClientFactory,IConfiguration configuration,AppDbContext context)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _context = context;

            var baseUrl = _configuration["HomeAssistant:BaseUrl"];
            var accessToken = _configuration["HomeAssistant:AccessToken"];

            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        }


        public async Task<Device?> AddDeviceAsync(AddDeviceToRoomDto dto)
        {
             var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Name.ToLower() == dto.RoomName.ToLower());

            if (room == null)
            {
              
                room = await _context.Rooms.FirstOrDefaultAsync(r => r.Name == "Default Room");

                if (room == null)
                {
                   
                    room = new Room { Name = "Default Room" };
                    _context.Rooms.Add(room);
                    await _context.SaveChangesAsync();
                }
            }

            var device = new Device
            {
                EntityId = dto.EntityId,
                CurrentState = dto.State,
                Room = room
            };

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            return device;
        }


        public async Task<List<Device>> GetDevicesByRoomNameAsync(string roomName)
        {
            return await _context.Devices
                .Where(d => d.Room.Name.ToLower() == roomName.ToLower())
                .ToListAsync();
        }

    }
}
