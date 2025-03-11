using Betting_Event_Maker.Enums;
using System.Text.Json.Serialization;

namespace Betting_Event_Maker.Models
{
    public class Event
    {
        private static readonly Random _random = new();
        private static (decimal, decimal, decimal) GenerateRandomChances()
        {
            decimal homeWin = (decimal)_random.NextDouble();
            decimal awayWin = (decimal)_random.NextDouble() * (1 - homeWin);
            decimal draw = 1 - homeWin - awayWin;
            return (homeWin, awayWin, draw);
        }
        private static readonly (decimal, decimal, decimal) _chances = GenerateRandomChances();

        public Guid Id { get; set; } = Guid.NewGuid();
        public required string EventName { get; set; }
        public required string HomeTeam { get; set; }
        public required string AwayTeam { get; set; }
        public decimal HomeWinChance { get; set; } = _chances.Item1;
        public decimal AwayWinChance { get; set; } = _chances.Item2;
        public decimal DrawChance { get; set; } = _chances.Item3;

        public required DateTime EventStartDate { get; set; }
        public required DateTime EventEndDate { get; set; }
        public List<string> EventSubscribers { get; set; } = new();

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EventResult EventResult { get; set; } = EventResult.Pending;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required EventType Type { get; set; }
    }
}
