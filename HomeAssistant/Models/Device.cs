using System.Text.Json.Serialization;

namespace HomeAssistant.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Status { get; set; }


        public Room Room { get; set; }


    }
}
