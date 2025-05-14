using HomeAssistant.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.Services
{
    public class RoomService
    {
        private readonly AppDbContext  _context;

        public RoomService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Room> AddRoomAsync(string roomName)
        {
            var room = new Room { Name = roomName };
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<bool> DeleteRoomAsync(string Name)
        {
            var room = await _context.Rooms.Include(r => r.Devices).FirstOrDefaultAsync(r => r.Name == Name);
            if (room == null)
                return false;

            
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
