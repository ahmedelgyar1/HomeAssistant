using Xunit;
using HomeAssistant.Models;
using HomeAssistant.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using HomeAssistant;

public class RoomServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_HomeAssistant_" + Guid.NewGuid())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddRoomAsync_ShouldAddRoomSuccessfully()
    {
        
        var context = GetInMemoryDbContext();
        var service = new RoomService(context);

     
        var result = await service.AddRoomAsync("Living Room");

      
        Assert.NotNull(result);
        Assert.Equal("Living Room", result.Name);

        var roomInDb = await context.Rooms.FirstOrDefaultAsync();
        Assert.NotNull(roomInDb);
        Assert.Equal("Living Room", roomInDb.Name);
    }

    [Fact]
    public async Task DeleteRoomAsync_ShouldDeleteRoomSuccessfully()
    {
        
        var context = GetInMemoryDbContext();
        var service = new RoomService(context);

       
        var room = new Room { Name = "Bedroom" };
        context.Rooms.Add(room);
        await context.SaveChangesAsync();

       
        var result = await service.DeleteRoomAsync("Bedroom");

       
        Assert.True(result);
        var deletedRoom = await context.Rooms.FirstOrDefaultAsync(r => r.Name == "Bedroom");
        Assert.Null(deletedRoom);
    }

    [Fact]
    public async Task DeleteRoomAsync_ShouldReturnFalseIfRoomNotFound()
    {
       
        var context = GetInMemoryDbContext();
        var service = new RoomService(context);

        
        var result = await service.DeleteRoomAsync("NonExistentRoom");

       
        Assert.False(result);
    }
}
