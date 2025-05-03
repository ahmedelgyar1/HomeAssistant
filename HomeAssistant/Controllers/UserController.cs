using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using HomeAssistant.Models;
using HomeAssistant.Services;
using HomeAssistant.DTOs;
using static System.Net.WebRequestMethods;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly IConfiguration _configuration;
    private readonly OtpService _otpService;
    private readonly EmailService _emailService;
    public AuthController(UserService userService, IConfiguration configuration,OtpService otpService , EmailService emailService)
    {
        _userService = userService;
        _configuration = configuration;
        _otpService = otpService;
        _emailService=emailService;
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
            Name = otpEntry.Name!,
            Email = otpEntry.Email,
            Password = otpEntry.Password!,
            BirthDate = otpEntry.BirthDate ?? DateTime.UtcNow
        };

        await _userService.AddUserAsync(user);

        return Ok("Registration completed successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User credentials)
    {
        var user = await _userService.LoginAsync(credentials.Email, credentials.Password);
        if (user == null)
            return Unauthorized("Invalid credentials.");

        
        var Token = _configuration["HomeAssistant:Token"];

        return Ok(new
        {
            message = "Login successful",
            homeAssistantToken = Token
        });
    }
}
