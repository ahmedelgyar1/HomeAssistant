using HomeAssistant.DTOs;
using HomeAssistant.Models;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace HomeAssistant.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly OtpService _otpService;
        private readonly EmailService _emailService;
        private readonly ILogger<UserService> _logger;

        public UserService(AppDbContext context, IConfiguration config, OtpService otpService, EmailService emailService, ILogger<UserService> logger)
        {
            _context = context;
            _config = config;
            _otpService = otpService;
            _emailService = emailService;
            _logger = logger;
        }
        /* public UserService(AppDbContext context, IConfiguration config, OtpService otpService, EmailService emailService)
        {
            _context = context;
            _config = config;
            _otpService = otpService;
            _emailService = emailService;
            
        }*/

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var emailExists = await IsEmailRegisteredAsync(dto.Email);
            if (emailExists)
                throw new InvalidOperationException("Email already registered");

            var code = await _otpService.GenerateOtpAsync(dto);
            await _emailService.SendEmailAsync(dto.Email, "Verify your email", $"Your code is: {code}");

            return code;
        }

        public async Task<bool> VerifyAsync(OtpVerificationDto dto)
        {
            var (isValid, otpEntry) = await _otpService.VerifyOtpAsync(dto.Email, dto.Code);

            if (!isValid || otpEntry == null)
                return false;

            var user = new User
            {
                Email = otpEntry.Email,
                Name = otpEntry.Name!,
                Password = otpEntry.Password!,
                IsApproved = false
            };

            await AddUserAsync(user);

            return true;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }

        public async Task<string> ForgotPasswordAsync(ForgotPasswordRequestDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                throw new InvalidOperationException("User not found");

            var code = await _otpService.GenerateForgotPasswordOtpAsync(dto.Email);
            await _emailService.SendEmailAsync(dto.Email, "Reset password for your email", $"Your code is: {code}");

            return code;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await GetByEmailAsync(dto.Email);

            if (user == null)
                throw new InvalidOperationException("User not found");

            user.Password = dto.NewPassword;
            await UpdateAsync(user);

            return true;
        }

        public async Task<List<User>> GetPendingUsersAsync()
        {
            return await _context.Users
                .Where(u => !u.IsApproved)
                .ToListAsync();
        }

        public async Task<List<User>> GetAcceptedUsersAsync()
        {
            return await _context.Users
                .Where(u => u.IsApproved)
                .ToListAsync();
        }

        public async Task<bool> ApproveUserAsync(string email)
        {
            var user = await GetByEmailAsync(email);
            if (user == null)
                throw new InvalidOperationException("User not found");

            user.IsApproved = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUserAsync(string email)
        {
            var user = await GetByEmailAsync(email);
            if (user == null)
                throw new InvalidOperationException("User not found");

            await DeleteAsync(user);

            return true;
        }

        public async Task<bool> ResendCodeAsync(string email)
        {
            var code = await _otpService.ResendCodeAsync(email);

            if (code == null)
                throw new InvalidOperationException("No code found for this email.");

            await _emailService.SendEmailAsync(email, "Verify your email", $"Your code is: {code}");

            return true;
        }

        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {

            try
            {
                _logger.LogInformation($"Fetching user with email: {email}");
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching user with email: {email}. Error: {ex.Message}");
                throw;
            }
        }
       

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task AddUsersAsync(List<User> users)
        {
            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();
        }
    }

}
