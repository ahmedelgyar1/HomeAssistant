namespace HomeAssistant.DTOs
{
    public class AddDeviceToRoomDto
    {
        public string EntityId { get; set; }
        public string State { get; set; }
        public string RoomName { get; set; }
    }
}
