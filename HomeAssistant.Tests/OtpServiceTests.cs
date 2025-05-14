using Xunit;
using HomeAssistant.Models;
using HomeAssistant.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using HomeAssistant;

public class OtpServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("Test_Otp_" + Guid.NewGuid())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GenerateForgotPasswordOtpAsync_ShouldGenerateAndStoreOtp()
    {
       
        var context = GetInMemoryDbContext();
        var service = new OtpService(context);
        string testEmail = "test@example.com";

      
        var resultCode = await service.GenerateForgotPasswordOtpAsync(testEmail);

        
        Assert.False(string.IsNullOrWhiteSpace(resultCode));

        var otpInDb = context.OtpEntries.FirstOrDefault(o => o.Email == testEmail && o.Code == resultCode);
        Assert.NotNull(otpInDb);
        Assert.Equal(testEmail, otpInDb.Email);
        Assert.True((DateTime.UtcNow - otpInDb.CreatedAt).TotalMinutes < 1);
    }
}
