using System.ComponentModel.DataAnnotations;

namespace Betting_Event_Maker.Dtos.Event
{
    public class EventDetailCreateDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "RoundNumber must be greater than 0.")]
        public int? RoundNumber { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "HomeTeamScore must be 0 or greater.")]
        public int? HomeTeamScore { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "AwayTeamScore must be 0 or greater.")]
        public int? AwayTeamScore { get; set; }
    }
}
