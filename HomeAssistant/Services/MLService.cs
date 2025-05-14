using HomeAssistant.DTOs;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeAssistant.Models;
namespace HomeAssistant.Services
{
    
    public class MLService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public MLService(HttpClient httpClient,IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
          
        }

        public async Task<PredictionResponseDto> GetPredictionsAsync(PredictionRequestDto input)
        {
            var json = JsonSerializer.Serialize(input);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://web-production-05f39.up.railway.app/predict", content);
            response.EnsureSuccessStatusCode();

            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PredictionResponseDto>(resultString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result;
        }
   
    }

}
