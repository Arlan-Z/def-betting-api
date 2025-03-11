using System.ComponentModel.DataAnnotations;

namespace Betting_Event_Maker.Dtos
{
    public class EventSubscribeDto
    {
        [Required]
        public string? CallbackUrl { get; set; }
    }
}
