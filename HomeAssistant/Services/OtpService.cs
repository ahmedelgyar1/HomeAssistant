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

        private string GenerateOtpCode()
        {
            return new Random().Next(10000000, 99999999).ToString();
        }

        public async Task<string> GenerateOtpAsync(RegisterDto dto)
        {
            var code = GenerateOtpCode();

            var otp = new OtpEntry
            {
                Email = dto.Email,
                Code = code,
                CreatedAt = DateTime.UtcNow,
                Name = dto.UserName,
                Password = dto.Password,
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

            if (otp == null )
                return (false, null);

            _context.OtpEntries.Remove(otp);
            await _context.SaveChangesAsync();

            return (true, otp);
        }

        public async Task<string> GenerateForgotPasswordOtpAsync(string email)
        {
            var code = GenerateOtpCode();

            var otp = new OtpEntry
            {
                Email = email,
                Code = code,
                CreatedAt = DateTime.UtcNow
            };

            _context.OtpEntries.Add(otp);
            await _context.SaveChangesAsync();

            return code;
        }

        public async Task<string?> ResendCodeAsync(string email)
        {
            var latestOtp = await _context.OtpEntries
                .Where(o => o.Email == email)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (latestOtp == null)
                return null;

      
            _context.OtpEntries.Remove(latestOtp);
            await _context.SaveChangesAsync();

          
            var newCode = GenerateOtpCode();

            var newOtp = new OtpEntry
            {
                Email = latestOtp.Email,
                Code = newCode,
                CreatedAt = DateTime.UtcNow,
                Name = latestOtp.Name,
                Password = latestOtp.Password
            };

            _context.OtpEntries.Add(newOtp);
            await _context.SaveChangesAsync();

            return newCode;
        }

    }
}
