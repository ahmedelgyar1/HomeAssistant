using HomeAssistant.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.Builders
{
    public class RoomBuilder
    {
        private Room _room;
        private readonly AppDbContext _appDbContext;

        public RoomBuilder(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public RoomBuilder WithName(string name)
        {
            _room = _appDbContext.Rooms
                .Include(r => r.Devices)
                .Include(r => r.CameraStreams)
                .FirstOrDefault(r => r.Name == name);

           
            if (_room == null)
            {
                _room = new Room { Name = name };
                _appDbContext.Rooms.Add(_room);
            }

            return this;
        }

        public RoomBuilder AddDevice(Device device)
        {
            _room.Devices.Add(device);
            return this;
        }

        public RoomBuilder AddCamera(CameraStream camera)
        {
            _room.CameraStreams.Add(camera);
            return this;
        }

        public async Task<Room> BuildAndSaveAsync()
        {
            await _appDbContext.SaveChangesAsync();
            return _room;
        }
    }

}
