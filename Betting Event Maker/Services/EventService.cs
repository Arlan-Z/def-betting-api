using Betting_Event_Maker.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Betting_Event_Maker.Dtos;

namespace Betting_Event_Maker.Services
{
    public class EventService
    {
        private readonly JsonFileService _jsonFileService;
        private readonly HttpClient _httpClient;

        public EventService(JsonFileService jsonFileService, HttpClient httpClient)
        {
            _jsonFileService = jsonFileService;
            _httpClient = httpClient;
        }

        public async Task UpdateEventsOdds(Event @event)
        {
            var eventDetails = await _jsonFileService.LoadEventDetailsAsync();
            var relatedEventDetails = eventDetails.Where(ed => ed.EventId == @event.Id).ToList();

            if (!relatedEventDetails.Any())
            {
                return;
            }

            int homeTotalScore = relatedEventDetails.Sum(ed => ed.HomeTeamScore);
            int awayTotalScore = relatedEventDetails.Sum(ed => ed.AwayTeamScore);

            decimal totalGoals = homeTotalScore + awayTotalScore;

            decimal initialHomeWinChance = @event.HomeWinChance;
            decimal initialAwayWinChance = @event.AwayWinChance;
            decimal initialDrawChance = @event.DrawChance;

            if (totalGoals == 0)
            {
                @event.HomeWinChance = initialHomeWinChance * 0.8m;
                @event.AwayWinChance = initialAwayWinChance * 0.8m;
                @event.DrawChance = initialDrawChance + (initialHomeWinChance * 0.2m) + (initialAwayWinChance * 0.2m);
            }
            else
            {
                decimal newHomeWinChance = ((decimal)homeTotalScore / totalGoals) * 100 * 0.7m + initialHomeWinChance * 0.3m;

                decimal newAwayWinChance = ((decimal)awayTotalScore / totalGoals) * 100 * 0.7m + initialAwayWinChance * 0.3m;

                decimal newDrawChance = 100 - newHomeWinChance - newAwayWinChance;

                if (Math.Abs(homeTotalScore - awayTotalScore) <= 1)
                {
                    newDrawChance += 10;
                    newHomeWinChance *= 0.9m;
                    newAwayWinChance *= 0.9m;
                }

                @event.HomeWinChance = newHomeWinChance;
                @event.AwayWinChance = newAwayWinChance;
                @event.DrawChance = newDrawChance;
            }

            decimal totalChance = @event.HomeWinChance + @event.AwayWinChance + @event.DrawChance;
            @event.HomeWinChance = (@event.HomeWinChance / totalChance) * 100;
            @event.AwayWinChance = (@event.AwayWinChance / totalChance) * 100;
            @event.DrawChance = (@event.DrawChance / totalChance) * 100;

            var events = await _jsonFileService.LoadEventsAsync();
            var eventToUpdate = events.FirstOrDefault(e => e.Id == @event.Id);
            if (eventToUpdate != null)
            {
                eventToUpdate.HomeWinChance = @event.HomeWinChance;
                eventToUpdate.AwayWinChance = @event.AwayWinChance;
                eventToUpdate.DrawChance = @event.DrawChance;
            }

            await _jsonFileService.SaveEventsAsync(events);
        }

        public async Task SendResultsToSubscribers(Event @event)
        {
            var payload = new EventResultDto
            {
                EventId = @event.Id,
                EventName = @event.EventName,
                Result = @event.EventResult.ToString(),
            };

            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            foreach (var subscriberUrl in @event.EventSubscribers)
            {
                try
                {
                    HttpResponseMessage response = await _httpClient.PostAsync(subscriberUrl, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Error Sending to {subscriberUrl}: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Sending to {subscriberUrl}: {ex.Message}");
                }
            }
        }
    }
}
