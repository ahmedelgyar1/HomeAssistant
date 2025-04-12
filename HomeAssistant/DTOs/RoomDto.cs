namespace HomeAssistant.DTOs
{
    public class RoomDto
    {
        public string Name { get; set; }
        public List<DeviceDto> Devices { get; set; } = new List<DeviceDto>();
        public List<CameraDto> Cameras { get; set; } = new List<CameraDto> { };
    }

  
    

}
