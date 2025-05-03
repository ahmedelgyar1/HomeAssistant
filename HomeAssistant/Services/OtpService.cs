using HomeAssistant.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.Services
{
    public class OtpService
    {
        private readonly AppDbContext _context;

        public OtpService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateOtpAsync(RegisterDto dto)
        {
            var code = new Random().Next(100000, 999999).ToString();

            var otp = new OtpEntry
            {
                Email = dto.Email,
                Code = code,
                CreatedAt = DateTime.UtcNow,
                Name = dto.FullName,
                Password = dto.Password,
                BirthDate = dto.BirthDate
            };

            _context.OtpEntries.Add(otp);
            await _context.SaveChangesAsync();

            return code;
        }

        public async Task<(bool, OtpEntry?)> VerifyOtpAsync(string email, string code)
        {
            var otp = await _context.OtpEntries
                .Where(o => o.Email == email && o.Code == code)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otp == null || DateTime.UtcNow - otp.CreatedAt > TimeSpan.FromMinutes(5))
                return (false, null);

            _context.OtpEntries.Remove(otp);
            await _context.SaveChangesAsync();

            return (true, otp);
        }

    }


}
