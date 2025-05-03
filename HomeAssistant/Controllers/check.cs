using Microsoft.AspNetCore.Mvc;

namespace HomeAssistant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeAssistantConnectionController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public HomeAssistantConnectionController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("check-connection")]
        public async Task<IActionResult> CheckConnection()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "http://192.168.1.7:8123/api/");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiIxNGJhYTE0ZDJmNTg0YWE5YjVjYmEwZGQxYWIyNDMxNCIsImlhdCI6MTc0NTc1ODQ1OCwiZXhwIjoyMDYxMTE4NDU4fQ.cUqHimOQpq2UCkUh7N2Mp2haf-pXG9BZSlkqGRXgfrk");

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return Ok("Connection to Home Assistant is successful!");
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Failed to connect to Home Assistant.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

    }

}
