namespace Betting_Event_Maker.Models
{
    public class PaymentCard
    {
        public string CardOwnerName { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public decimal AvailableAmount { get; set; }
    }
}
