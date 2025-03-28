using Betting_Event_Maker.Models;

namespace Betting_Event_Maker.Services
{
    public class EventClosingBackgroundService : BackgroundService
    {
        private readonly ILogger<EventClosingBackgroundService> _logger;
        private readonly IServiceProvider _services;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);
        private readonly IHttpClientFactory _httpClientFactory;

        public EventClosingBackgroundService(
            ILogger<EventClosingBackgroundService> logger,
            IServiceProvider services,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _services = services;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EventClosingBackgroundService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CloseExpiredEventsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while closing expired events.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("EventClosingBackgroundService is stopping.");
        }

        private async Task CloseExpiredEventsAsync()
        {
            using (var scope = _services.CreateScope())
            {
                var jsonFileService = scope.ServiceProvider.GetRequiredService<JsonFileService>();

                var events = await jsonFileService.LoadEventsAsync();
                var eventsToClose = new List<Event>();

                foreach (var @event in events)
                {
                    if (@event.EventResult == Enums.EventResult.Pending && @event.EventEndDate <= DateTime.Now)
                    {
                        eventsToClose.Add(@event);
                    }
                }
                if (eventsToClose.Count == 0) return;

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("http://localhost:5283"); // TODO: change to launchsetting args
                foreach (var eventToClose in eventsToClose)
                {

                    try
                    {
                        var url = $"api/events/{eventToClose.Id}/end";

                        var response = await client.PostAsync(url, null); 
                                                                          
                        if (response.IsSuccessStatusCode)
                        {
                            _logger.LogInformation($"Event {eventToClose.Id} closed successfully via API.");

                        }
                        else
                        {
                            _logger.LogError($"Failed to close event {eventToClose.Id} via API. Status code: {response.StatusCode}");
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        _logger.LogError(ex, $"Error closing event {eventToClose.Id} via API.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"An unexpected error occurred while closing event {eventToClose.Id} via API.");
                    }
                }
            }
        }
    }
}