namespace HomeAssistant.Models
{
    public class OtpEntry
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }

     
        public string? Name { get; set; }
        public string? Password { get; set; }
       
    }


}
