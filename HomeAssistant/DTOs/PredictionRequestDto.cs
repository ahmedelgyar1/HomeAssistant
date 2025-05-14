namespace HomeAssistant.DTOs
{
    public class PredictionRequestDto
    {
        public string mood { get; set; }
        public string person_condition { get; set; }
        public string time_of_day { get; set; }
        public int at_home { get; set; }
        public int is_holiday { get; set; }
    }
}
