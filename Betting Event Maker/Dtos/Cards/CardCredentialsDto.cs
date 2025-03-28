using System.ComponentModel.DataAnnotations;

namespace Betting_Event_Maker.Dtos.Cards
{
    public class CardCredentialsDto
    {
        [Required]
        public string CardOwnerName { get; set; }

        [Required]
        public string CardNumber { get; set; }

        [Required]
        public string CVV { get; set; }
    }
}
