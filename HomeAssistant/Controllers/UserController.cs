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
    public AuthController(UserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;

    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try
        {
            var code = await _userService.RegisterAsync(dto);
            return Ok("Code sent to your email.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    /*[HttpPost("register")]
public async Task<IActionResult> Register(RegisterDto dto)
{
var emailExists = await _userService.IsEmailRegisteredAsync(dto.Email);
if (emailExists)
return BadRequest("Email already registered");
var code = await _otpService.GenerateOtpAsync(dto);
await _emailService.SendEmailAsync(dto.Email, "Verify your email", $"Your code is: {code}");
return Ok("Code sent to your email.");
    
     */
    [HttpPost("verify")]
    public async Task<IActionResult> Verify(OtpVerificationDto dto)
    {
        var isValid = await _userService.VerifyAsync(dto);

        if (!isValid)
            return BadRequest("Invalid or expired code");

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
        try
        {
            await _userService.ForgotPasswordAsync(dto);
            return Ok("Code sent to your email.");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var success = await _userService.ResetPasswordAsync(dto);

        if (!success)
            return NotFound("User not found.");

        return Ok("Password has been reset successfully.");
    }

    [HttpGet("pending-users")]
    public async Task<IActionResult> GetPendingUsers()
    {
        var pendingUsers = await _userService.GetPendingUsersAsync();
        return Ok(pendingUsers);
    }

    [HttpGet("accepted-users")]
    public async Task<IActionResult> GetAcceptedUsers()
    {
        var acceptedUsers = await _userService.GetAcceptedUsersAsync();
        return Ok(acceptedUsers);
    }

    [HttpPost("approve-user/{Email}")]
    public async Task<IActionResult> ApproveUser(string Email)
    {
        var success = await _userService.ApproveUserAsync(Email);

        if (!success)
            return NotFound("User not found.");

        return Ok("User approved successfully.");
    }

    [HttpPost("resend-code")]
    public async Task<IActionResult> ResendCode([FromBody] ResendCodeDto dto)
    {
        try
        {
            await _userService.ResendCodeAsync(dto.Email);
            return Ok("Code resent successfully.");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("delete-user/{Email}")]
    public async Task<IActionResult> DeleteUser(string Email)
    {
        var success = await _userService.DeleteUserAsync(Email);

        if (!success)
            return NotFound("User not found");

        return Ok("User deleted successfully.");
    }

   
}
