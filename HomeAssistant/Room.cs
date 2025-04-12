namespace HomeAssistant
{
    public class Room
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ICollection<Device> Devices { get; set; }= new List<Device>();
        public ICollection<CameraStream> CameraStreams { get; set; } = new List<CameraStream>();

    }
}
