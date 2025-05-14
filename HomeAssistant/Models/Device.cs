using System.Text.Json.Serialization;

namespace HomeAssistant.Models
{
    public class Device
    {
        public int Id { get; set; } 

        [JsonPropertyName("entity_id")]
        public string EntityId { get; set; }

        [JsonPropertyName("state")]
        public string CurrentState { get; set; }

        public Room Room { get; set; } 


    }
}
