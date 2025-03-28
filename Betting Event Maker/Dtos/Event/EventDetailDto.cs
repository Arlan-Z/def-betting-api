using Betting_Event_Maker.Enums;

namespace Betting_Event_Maker.Dtos.Event
{
    public class EventDetailDto
    {
        public Guid Id { get; set; }
        public required string EventName { get; set; }
        public required string HomeTeam { get; set; }
        public required string AwayTeam { get; set; }
        public decimal Odds { get; set; }
        public required DateTime EventStartDate { get; set; }
        public required DateTime EventEndDate { get; set; }
        public required string EventResult { get; set; }
        public required string Type { get; set; }
        public required decimal HomeWinChance { get; set; }
        public required decimal AwayWinChance { get; set; }
        public required decimal DrawChance { get; set; }
        public List<EventRoundDto> EventRounds { get; set; } = new();
    }

    public class EventRoundDto
    {
        public int RoundNumber { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public DateTime RoundDateTime { get; set; }
    }
}