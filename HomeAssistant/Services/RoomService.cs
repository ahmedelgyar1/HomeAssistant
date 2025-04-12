namespace HomeAssistant.Services
{
    public class RoomService
    {
        private readonly AppDbContext _context;
        public RoomService(AppDbContext context)
        {
            _context = context;
        }

    }
}
