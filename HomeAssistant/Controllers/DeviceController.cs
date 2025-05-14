using HomeAssistant.DTOs;
using HomeAssistant.Models;
using HomeAssistant.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class HomeAssistantController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly DeviceService _deviceService;

    public HomeAssistantController( IHttpClientFactory httpClientFactory, IConfiguration configuration,DeviceService deviceService)
    {
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
        _deviceService = deviceService;

        var baseUrl = configuration["HomeAssistant:BaseUrl"];
        var accessToken = configuration["HomeAssistant:AccessToken"];
        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    }


    [HttpPost("turn-on")]
    public async Task<IActionResult> TurnOnDevice([FromBody] DeviceControlDto dto)
    {
        var json = $"{{\"entity_id\":\"{dto.EntityId}\"}}";
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/services/homeassistant/turn_on", content);

        if (response.IsSuccessStatusCode)
            return Ok($"{dto.EntityId} turned on successfully");
        else
            return StatusCode((int)response.StatusCode, $"Failed to turn on {dto.EntityId}");
    }

    [HttpPost("turn-off")]
    public async Task<IActionResult> TurnOffDevice([FromBody] DeviceControlDto dto)
    {
        var json = $"{{\"entity_id\":\"{dto.EntityId}\"}}";
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/services/homeassistant/turn_off", content);

        if (response.IsSuccessStatusCode)
            return Ok($"{dto.EntityId} turned off successfully");
        else
            return StatusCode((int)response.StatusCode, $"Failed to turn off {dto.EntityId}");
    }
    [HttpGet("Get-Devices")]
    public async Task<IActionResult> GetAllStates()
    {
        var response = await _httpClient.GetAsync("api/states");

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, "Failed to retrieve Devices");

        var jsonString = await response.Content.ReadAsStringAsync();

        var states = JsonSerializer.Deserialize<List<Device>>(jsonString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return Ok(states);
    }

    [HttpPost("add-device-to-room")]
    public async Task<IActionResult> AddDevice([FromBody] AddDeviceToRoomDto dto)
    {
        var result = await _deviceService.AddDeviceAsync(dto);

        if (result == null)
            return NotFound($"Room with Name {dto.RoomName} not found.");

        return Ok(new
        {
            message = "Device added to room successfully.",
            deviceId = result.Id
        });
    }

    [HttpGet("room/name/{roomName}/devices")]
    public async Task<IActionResult> GetDevicesByRoomName(string roomName)
    {
        var devices = await _deviceService.GetDevicesByRoomNameAsync(roomName);

        if (devices == null || !devices.Any())
            return NotFound($"No devices found for room with name '{roomName}'");

        return Ok(devices);
    }


}
