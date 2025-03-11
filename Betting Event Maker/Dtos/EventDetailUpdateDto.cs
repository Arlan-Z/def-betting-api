using System.ComponentModel.DataAnnotations;

namespace Betting_Event_Maker.Dtos
{
    public class EventDetailUpdateDto
    {
        [Required]
        public int? RoundNumber { get; set; }

        [Required]
        public int? HomeTeamScore { get; set; }

        [Required]
        public int? AwayTeamScore { get; set; }
    }
}
