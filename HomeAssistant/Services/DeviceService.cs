using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
namespace HomeAssistant.Services
{
    public class DeviceService
    {
        private readonly HttpClient _client;

        public DeviceService()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://192.168.1.13:8123/");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer YOUR_LONG_LIVED_ACCESS_TOKEN");
        }

        public async Task TurnOffTVAsync()
        {
            var json = "{\"entity_id\":\"media_player.50_crystal_uhd\"}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/services/media_player/turn_off", content);
            response.EnsureSuccessStatusCode();
        }


    }
}
