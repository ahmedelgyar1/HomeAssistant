using HomeAssistant.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using HomeAssistant.Services;

namespace HomeAssistant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmartHomeController : ControllerBase
    {
        private readonly MLService _mlService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public SmartHomeController(MLService mlService,IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _mlService = mlService;
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            var baseUrl = configuration["HomeAssistant:BaseUrl"];
            var accessToken = configuration["HomeAssistant:AccessToken"];
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        }

        [HttpPost("get-recommendations")]
        public async Task<IActionResult> GetRecommendations([FromBody] PredictionRequestDto dto)
        {
            try
            {
                var result = await _mlService.GetPredictionsAsync(dto);

                if (result == null || result.Recommendations == null)
                    return StatusCode(500, "Failed to get predictions from model.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpPost("predict-and-act")]
        public async Task<IActionResult> PredictAndAct([FromBody] PredictionRequestDto dto)
        {
            var modelResult = await _mlService.GetPredictionsAsync(dto);
            var recommendationsString = string.Join(", ", modelResult.Recommendations);


            if (recommendationsString.ToLower().Contains("turn on tv"))
            {
                var json = $"{{\"entity_id\":\"{"media_player.lg_webos_tv_e74b"}\"}}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/services/homeassistant/turn_on", content);

                if (response.IsSuccessStatusCode)
                    return Ok($"{"dto.EntityId"} turned on successfully");
                else
                    return StatusCode((int)response.StatusCode, $"Failed to turn on {"media_player.lg_webos_tv_e74b"}");
            }

            else if (recommendationsString.ToLower().Contains("turn off tv"))
            {
                var json = $"{{\"entity_id\":\"{"media_player.lg_webos_tv_e74b"}\"}}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/services/homeassistant/turn_off", content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to turn off TV. Status: " + response.StatusCode);
                }
            }

            return Ok(new { prediction = modelResult, actionTriggered = true });


        }

    }
}
