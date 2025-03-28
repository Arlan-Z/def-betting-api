using System.ComponentModel.DataAnnotations;

namespace Betting_Event_Maker.Dtos.Cards
{
    public class CardAddMoneyDto
    {
        [Required]
        public string CardNumber { get; set; }

        [Required]
        [PositiveNonZeroDecimal]
        public Decimal PaymentAmount { get; set; }
    }
}
