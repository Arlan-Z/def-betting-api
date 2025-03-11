using Betting_Event_Maker.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Betting_Event_Maker.Services
{
    public class JsonFileService
    {
        private readonly string _eventsFilePath = "Database/Events.json";
        private readonly string _eventDetailsFilePath = "Database/EventDetails.json";

        public async Task<List<Event>> LoadEventsAsync()
        {
            return await LoadFromFileAsync<Event>(_eventsFilePath);
        }

        public async Task SaveEventsAsync(List<Event> events)
        {
            await SaveToFileAsync(_eventsFilePath, events);
        }

        public async Task<List<EventDetail>> LoadEventDetailsAsync()
        {
            return await LoadFromFileAsync<EventDetail>(_eventDetailsFilePath);
        }

        public async Task SaveEventDetailsAsync(List<EventDetail> eventDetails)
        {
            await SaveToFileAsync(_eventDetailsFilePath, eventDetails);
        }

        private async Task<List<T>> LoadFromFileAsync<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            try
            {
                string json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
                {
                    Converters = { new DateTimeConverterUsingDateTimeParse() },
                    PropertyNameCaseInsensitive = true
                }) ?? new List<T>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON from {filePath}: {ex.Message}");
                return new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data from {filePath}: {ex.Message}");
                return new List<T>();
            }
        }

        private async Task SaveToFileAsync<T>(string filePath, List<T> data)
        {
            try
            {
                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new DateTimeConverterUsingDateTimeParse() }
                });
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data to {filePath}: {ex.Message}");
            }
        }
    }

    public class DateTimeConverterUsingDateTimeParse : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (DateTime.TryParse(reader.GetString(), out DateTime date))
            {
                return date;
            }
            return default;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss"));
        }
    }
}
