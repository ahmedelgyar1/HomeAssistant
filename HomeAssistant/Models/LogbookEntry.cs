namespace HomeAssistant.Models
{
    public class LogbookEntry
    {
        public int Id { get; set; }

        public string EntityId { get; set; }
        public DateTime When { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
    }
}
