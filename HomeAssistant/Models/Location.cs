namespace HomeAssistant.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string Apartment { get; set; }
        public string StreetAddress { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
    }
}
