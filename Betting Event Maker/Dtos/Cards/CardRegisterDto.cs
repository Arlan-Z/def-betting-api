using System.ComponentModel.DataAnnotations;

namespace Betting_Event_Maker.Dtos.Cards
{
    public class CardRegisterDto
    {
        [Required(ErrorMessage = "Username is needed")]
        public string CardOwnerName { get; set; }

        public decimal? AvailableAmount { get; set; }
    }
}
