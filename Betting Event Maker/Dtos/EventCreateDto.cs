using Betting_Event_Maker.Enums;
using System.ComponentModel.DataAnnotations;

namespace Betting_Event_Maker.Dtos
{
    public class EventCreateDto
    {
        [Required]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public EventType? Type { get; set; }

        [Required]
        public string HomeTeam { get; set; } = string.Empty;

        [Required]
        public string AwayTeam { get; set; } = string.Empty;

        //[Required]
        //[OddsRange]
        //public decimal Odds { get; set; }

        public DateTime? EventStartDate { get; set; }

        [Required]
        public DateTime? EventEndDate { get; set; }

        public List<string> EventSubscribers { get; set; } = new();
    }

    public class OddsRangeAttribute : ValidationAttribute
    {
        private readonly decimal _min = 0.01m;
        private readonly decimal _max = 0.99m;

        public override bool IsValid(object? value)
        {
            if (value is decimal decimalValue)
            {
                return decimalValue >= _min && decimalValue <= _max;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be between {_min} and {_max} (inclusive).";
        }
    }
}
