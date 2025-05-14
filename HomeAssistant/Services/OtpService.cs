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

           /* var otp = new OtpEntry
            {
                Email = dto.Email,
                Code = code,
                CreatedAt = DateTime.UtcNow,
                Name = dto.UserName,
                Password = dto.Password,
            };*/
           var otp= CreateOtp(dto.Email,code,dto.UserName,dto.Password);
            _context.OtpEntries.Add(otp);
            await _context.SaveChangesAsync();

            return code;
        }

        /* public async Task<(bool, OtpEntry?)> VerifyOtpAsync(string email, string code)
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
*/
        public async Task<(bool, OtpEntry?)> VerifyOtpAsync(string email, string code)
        {
            var otp = await GetValidOtpAsync(email, code);
            if (otp == null)
                return (false, null);

            await RemoveOtpAsync(otp);
            return (true, otp);
        }

        public async Task<OtpEntry?> GetValidOtpAsync(string email, string code)
        {
            return await _context.OtpEntries
                .Where(o => o.Email == email && o.Code == code)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task RemoveOtpAsync(OtpEntry otp)
        {
            _context.OtpEntries.Remove(otp);
            await _context.SaveChangesAsync();
        }
        private OtpEntry CreateOtp(string email, string code, string? name = null, string? password = null)
        {
            return new OtpEntry
            {
                Email = email,
                Code = code,
                CreatedAt = DateTime.UtcNow,
                Name = name,
                Password = password
            };
        }

        public async Task<string> GenerateForgotPasswordOtpAsync(string email)
        {
            var code = GenerateOtpCode();

            var otp = CreateOtp(email, code);

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

            var newOtp = CreateOtp(latestOtp.Email, newCode, latestOtp.Name, latestOtp.Password);

            _context.OtpEntries.Add(newOtp);
            await _context.SaveChangesAsync();

            return newCode;
        }

    }
}
