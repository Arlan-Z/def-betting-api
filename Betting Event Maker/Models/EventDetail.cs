namespace Betting_Event_Maker.Models
{
    public class EventDetail
    {
        public Guid Id { get; set; } = Guid.NewGuid(); 
        public Guid EventId { get; set; } 
        public int RoundNumber { get; set; } 
        public int HomeTeamScore { get; set; } 
        public int AwayTeamScore { get; set; } 
        public DateTime RoundDateTime { get; set; } = DateTime.UtcNow; 
    }
}
