namespace HomeAssistant.Services
{
    public class DeviceService
    {
        private readonly AppDbContext _context;
        public DeviceService(AppDbContext context)
        {
            _context = context;
        }

    }
}
