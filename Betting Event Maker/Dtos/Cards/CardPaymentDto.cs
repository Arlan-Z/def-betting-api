using System.ComponentModel.DataAnnotations;

namespace Betting_Event_Maker.Dtos.Cards
{
    public class CardPaymentDto
    {
        [Required]
        public string CardOwnerName { get; set; }

        [Required]
        public string CardNumber { get; set; }

        [Required]
        public string CVV { get; set; }

        [Required]
        [PositiveNonZeroDecimal]
        public Decimal PaymentAmount { get; set; }

    }

    internal class PositiveNonZeroDecimalAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is Decimal decimalValue)
            {
                return decimalValue > 0;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be a non-zero value.";
        }
    }
}
