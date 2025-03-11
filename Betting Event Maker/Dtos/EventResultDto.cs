using Betting_Event_Maker.Enums;

namespace Betting_Event_Maker.Dtos
{
    public class EventResultDto
    {
        public required Guid EventId { get; set; }
        public required string EventName { get; set; }
        public required string Result { get; set; }
        //public required int HomeScore { get; set; }
        //public required int AwayScore { get; set; }
    }
}
