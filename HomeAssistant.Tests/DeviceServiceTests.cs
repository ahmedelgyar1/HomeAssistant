using Xunit;
using HomeAssistant.Models;
using HomeAssistant.DTOs;
using HomeAssistant.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using HomeAssistant;

public class DeviceServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_DeviceService_" + Guid.NewGuid())
            .Options;

        return new AppDbContext(options);
    }

    private DeviceService GetDeviceService(AppDbContext context)
    {
       
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var httpClient = new HttpClient();
        httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["HomeAssistant:BaseUrl"]).Returns("http://localhost/");
        configMock.Setup(c => c["HomeAssistant:AccessToken"]).Returns("fake_token");

        return new DeviceService(httpClientFactoryMock.Object, configMock.Object, context);
    }

    [Fact]
    public async Task AddDeviceAsync_ShouldAddDeviceToRoom()
    {
       
        var context = GetInMemoryDbContext();
        var room = new Room { Name = "Living Room" };
        context.Rooms.Add(room);
        await context.SaveChangesAsync();

        var service = GetDeviceService(context);
        var dto = new AddDeviceToRoomDto
        {
            RoomName = room.Name,
            EntityId = "light.living_room",
            State = "off"
        };

        
        var result = await service.AddDeviceAsync(dto);

      
        Assert.NotNull(result);
        Assert.Equal("light.living_room", result.EntityId);
        Assert.Equal("off", result.CurrentState);
        Assert.NotNull(result.Room);
        Assert.Equal(room.ID, result.Room.ID);
    }

    [Fact]
    public async Task GetDevicesByRoomIdAsync_ShouldReturnDevicesInRoom()
    {
       
        var context = GetInMemoryDbContext();
        var room = new Room { Name = "Bedroom" };
        var device1 = new Device { EntityId = "light.bedroom", CurrentState = "on", Room = room };
        var device2 = new Device { EntityId = "fan.bedroom", CurrentState = "off", Room = room };

        context.Rooms.Add(room);
        context.Devices.AddRange(device1, device2);
        await context.SaveChangesAsync();

        var service = GetDeviceService(context);

       
        var result = await service.GetDevicesByRoomNameAsync(room.Name);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, d => d.EntityId == "light.bedroom");
        Assert.Contains(result, d => d.EntityId == "fan.bedroom");
    }
}
