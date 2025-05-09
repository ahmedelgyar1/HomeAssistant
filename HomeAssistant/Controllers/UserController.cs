using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using HomeAssistant.Models;
using HomeAssistant.Services;
using HomeAssistant.DTOs;
using static System.Net.WebRequestMethods;
using HomeAssistant;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly IConfiguration _configuration;
    private readonly OtpService _otpService;
    private readonly EmailService _emailService;
    private readonly AppDbContext _context;
    public AuthController(UserService userService, IConfiguration configuration,OtpService otpService , EmailService emailService , AppDbContext context)
    {
        _userService = userService;
        _configuration = configuration;
        _otpService = otpService;
        _emailService=emailService;
        _context = context;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var emailExists = await _userService.IsEmailRegisteredAsync(dto.Email);
        if (emailExists)
            return BadRequest("Email already registered");

        var code = await _otpService.GenerateOtpAsync(dto);

        await _emailService.SendEmailAsync(dto.Email, "Verify your email", $"Your code is: {code}");

        return Ok("Code sent to your email.");
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify(OtpVerificationDto dto)
    {
        var (isValid, otpEntry) = await _otpService.VerifyOtpAsync(dto.Email, dto.Code);

        if (!isValid || otpEntry == null)
            return BadRequest("Invalid or expired code");

        var user = new User
        {
           
            Email = otpEntry.Email,
            Name = otpEntry.Name!,
            Password = otpEntry.Password!,
            IsApproved = false

        };

        await _userService.AddUserAsync(user);

        return Ok("Registration completed successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        var user = await _userService.LoginAsync(login.Email, login.Password);
        if (user == null)
            return Unauthorized("Invalid credentials.");

        if (!user.IsApproved)
            return Forbid("Account is pending approval by admin."); 

        var token = _configuration["HomeAssistant:Token"];

        return Ok(new
        {
            message = "Login successful",
            homeAssistantToken = token
        });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
            return NotFound("User not found");

        var code = await _otpService.GenerateForgotPasswordOtpAsync(dto.Email);

        await _emailService.SendEmailAsync(dto.Email, "reset password for your email", $"Your code is: {code}");
        return Ok("Code sent to your email.");
    }

    [HttpPost("verify-reset-code")]
    public async Task<IActionResult> VerifyResetCode([FromBody] OtpVerificationDto dto)
    {
        var (isValid, otpEntry) = await _otpService.VerifyOtpAsync(dto.Email, dto.Code);

        if (!isValid || otpEntry == null)
            return BadRequest("Invalid or expired code.");

        return Ok("Code verified. You can now reset your password.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var user = await _userService.GetByEmailAsync(dto.Email);

        if (user == null)
            return NotFound("User not found.");

        user.Password = dto.NewPassword;
        await _userService.UpdateAsync(user);

        return Ok("Password has been reset successfully.");
    }
    [HttpGet("pending-users")]
    public async Task<IActionResult> GetPendingUsers()
    {
        var pendingUsers = await _context.Users
            .Where(u => !u.IsApproved)
            .ToListAsync();

        return Ok(pendingUsers);
    }
    [HttpPost("approve-user")]
    public async Task<IActionResult> ApproveUser([FromBody] ApproveUserDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
            return NotFound("User not found.");

        user.IsApproved = true;
        await _context.SaveChangesAsync();

        return Ok("User approved successfully.");
    }

    [HttpPost("resend-code")]
    public async Task<IActionResult> ResendCode([FromBody] ResendCodeDto dto)
    {
        var code = await _otpService.ResendCodeAsync(dto.Email);

        if (code == null)
            return NotFound("No code found for this email.");

        await _emailService.SendEmailAsync(dto.Email, "Verify your email", $"Your code is: {code}");

        return Ok(new { message = "Code resent successfully.", code });
    }
}
