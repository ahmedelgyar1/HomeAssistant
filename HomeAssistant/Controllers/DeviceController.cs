using HomeAssistant.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeAssistantController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public HomeAssistantController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();

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
    }
}
