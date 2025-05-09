using HomeAssistant.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeAssistantController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HomeAssistantController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;

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
        [HttpGet("devices")]
        public async Task<IActionResult> GetDevices()
        {
            var accessToken = _configuration["HomeAssistant:AccessToken"];
            var baseUrl = _configuration["HomeAssistant:BaseUrl"];

            if (string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(accessToken))
                return BadRequest("Home Assistant configuration is missing.");

            var wsUri = baseUrl.Replace("https://", "wss://").TrimEnd('/') + "/api/websocket";

            using var socket = new ClientWebSocket();

            try
            {
                await socket.ConnectAsync(new Uri(wsUri), CancellationToken.None);

                var buffer = new byte[8192];

                await ReceiveFullMessage(socket, buffer); // auth_required
                await Send(socket, $"{{ \"type\": \"auth\", \"access_token\": \"{accessToken}\" }}");
                await ReceiveFullMessage(socket, buffer); // auth_ok

                var deviceRequest = new
                {
                    id = 1,
                    type = "config/device_registry/list"
                };

                var jsonRequest = JsonSerializer.Serialize(deviceRequest);
                await Send(socket, jsonRequest);

                var jsonResponse = await ReceiveFullMessage(socket, buffer);

                var parsed = JsonDocument.Parse(jsonResponse);

                if (parsed.RootElement.TryGetProperty("result", out var result))
                {
                    return Ok(result);
                }

                return BadRequest("Failed to retrieve devices.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error connecting to Home Assistant: {ex.Message}");
            }
        }
        private async Task<string> ReceiveFullMessage(ClientWebSocket socket, byte[] buffer)
        {
            var result = new WebSocketReceiveResult(0, WebSocketMessageType.Text, false);
            var fullMessage = new StringBuilder();

            do
            {
                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                fullMessage.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
            }
            while (!result.EndOfMessage);

            return fullMessage.ToString();
        }


        private async Task Send(ClientWebSocket socket, string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task<string> Receive(ClientWebSocket socket, byte[] buffer)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }
    }
}
