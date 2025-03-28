using Betting_Event_Maker.Dtos.Event;
using Betting_Event_Maker.Models;
using Betting_Event_Maker.Services;
using Microsoft.AspNetCore.Mvc;

namespace Betting_Event_Maker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly JsonFileService _jsonFileService;
        private readonly EventService _eventService;

        public EventsController(JsonFileService jsonFileService, EventService eventService)
        {
            _jsonFileService = jsonFileService;
            _eventService = eventService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> ListAllEvents()
        {
            var events = await _jsonFileService.LoadEventsAsync();
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventAsync([FromRoute] string id)
        {
            var events = await _jsonFileService.LoadEventsAsync();
            var eventItem = events.FirstOrDefault(e => e.Id.ToString() == id);

            return eventItem == null ? NotFound($"Event with ID {id} not found.") : Ok(eventItem);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateEventAsync([FromBody] EventCreateDto eventCreateDto)
        {
            var events = await _jsonFileService.LoadEventsAsync();

            var newEvent = new Event
            {
                EventName = eventCreateDto.EventName,
                Type = eventCreateDto.Type ?? Enums.EventType.Other,
                HomeTeam = eventCreateDto.HomeTeam,
                AwayTeam = eventCreateDto.AwayTeam,
                EventStartDate = eventCreateDto.EventStartDate ?? DateTime.Now,
                EventEndDate = eventCreateDto.EventEndDate ?? DateTime.Now,
                EventSubscribers = eventCreateDto.EventSubscribers ?? new List<string>()
            };

            Console.WriteLine($"New event ID: {newEvent.Id}");

            events.Add(newEvent);
            await _jsonFileService.SaveEventsAsync(events);

            return CreatedAtAction(nameof(GetEventAsync), new { id = newEvent.Id.ToString() }, newEvent);
        }

        [HttpPost("{id}/end")]
        public async Task<IActionResult> EndEvent([FromRoute] string id)
        {
            var events = await _jsonFileService.LoadEventsAsync();
            var eventItem = events.FirstOrDefault(e => e.Id.ToString() == id);

            if (eventItem == null)
            {
                return NotFound($"Event with ID {id} not found.");
            }

            if (eventItem.EventResult != Enums.EventResult.Pending)
            {
                return BadRequest($"Event with ID {id} is not active.");
            }

            var eventDetails = await _jsonFileService.LoadEventDetailsAsync();
            var eventRounds = eventDetails.Where(e => e.EventId.ToString() == id).ToList();

            int homeTotalScore = 0;
            int awayTotalScore = 0;

            if (eventRounds.Any())
            {
                homeTotalScore = eventRounds.Sum(e => e.HomeTeamScore);
                awayTotalScore = eventRounds.Sum(e => e.AwayTeamScore);
            }
            else
            {
                return await CancelEvent(id);
            }

            if (homeTotalScore > awayTotalScore)
                eventItem.EventResult = Enums.EventResult.HomeWin;
            else if (awayTotalScore > homeTotalScore)
                eventItem.EventResult = Enums.EventResult.AwayWin;
            else
                eventItem.EventResult = Enums.EventResult.Draw;

            await _jsonFileService.SaveEventsAsync(events);
            await _eventService.SendResultsToSubscribers(eventItem);

            return Ok(new
            {
                eventItem.Id,
                eventItem.EventName,
                HomeScore = homeTotalScore,
                AwayScore = awayTotalScore,
                Result = eventItem.EventResult.ToString()
            });
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelEvent([FromRoute] string id)
        {
            var events = await _jsonFileService.LoadEventsAsync();
            var eventItem = events.FirstOrDefault(e => e.Id.ToString() == id);

            if (eventItem == null) return NotFound($"Event with ID {id} not found.");

            eventItem.EventResult = Enums.EventResult.Canceled;

            await _jsonFileService.SaveEventsAsync(events);
            await _eventService.SendResultsToSubscribers(eventItem);

            return Ok($"Event with ID {id} has been canceled.");
        }

        //[HttpPatch("{id}/update")]
        //public IActionResult UpdateEvent([FromRoute] string id)
        //{
        //    throw new NotImplementedException();
        //}

        [HttpPost("{id}/subscribe")]
        public async Task<IActionResult> SubscribeToEvent([FromRoute] string id, [FromBody] EventSubscribeDto eventSubDto)
        {
            var events = await _jsonFileService.LoadEventsAsync();
            var eventItem = events.FirstOrDefault(e => e.Id.ToString() == id);

            if (eventItem == null)
            {
                return NotFound($"Event with ID {id} not found.");
            }

            if (eventItem.EventResult != Enums.EventResult.Pending)
            {
                return BadRequest($"Event with Id {id} is Not Active");
            }

            if (eventItem.EventSubscribers.Contains(eventSubDto.CallbackUrl))
            {
                return BadRequest("This callback URL is already subscribed.");
            }

            eventItem.EventSubscribers.Add(eventSubDto.CallbackUrl);
            await _jsonFileService.SaveEventsAsync(events);

            return Ok($"Successfully subscribed to event {id}.");
        }

        [HttpDelete("{id}/delete")]
        public async Task<IActionResult> DeleteEvent([FromRoute] string id)
        {
            var events = await _jsonFileService.LoadEventsAsync();
            var eventItem = events.FirstOrDefault(e => e.Id.ToString() == id);

            if (eventItem == null)
            {
                return NotFound($"Event with ID {id} not found.");
            }

            events.Remove(eventItem);
            await _jsonFileService.SaveEventsAsync(events);

            var eventDetails = await _jsonFileService.LoadEventDetailsAsync();
            var updatedDetails = eventDetails.Where(e => e.EventId.ToString() != id).ToList();
            await _jsonFileService.SaveEventDetailsAsync(updatedDetails);

            return NoContent();
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetEventDetails([FromRoute] string id)
        {
            var events = await _jsonFileService.LoadEventsAsync();
            var eventItem = events.FirstOrDefault(e => e.Id.ToString() == id);

            if (eventItem == null)
            {
                return NotFound($"Event with ID {id} not found.");
            }

            var eventDetails = await _jsonFileService.LoadEventDetailsAsync();
            var relatedRounds = eventDetails
                .Where(e => e.EventId.ToString() == id)
                .Select(r => new EventRoundDto
                {
                    RoundNumber = r.RoundNumber,
                    HomeTeamScore = r.HomeTeamScore,
                    AwayTeamScore = r.AwayTeamScore,
                    RoundDateTime = r.RoundDateTime
                })
                .ToList();

            var eventDto = new EventDetailDto
            {
                Id = eventItem.Id,
                EventName = eventItem.EventName,
                HomeTeam = eventItem.HomeTeam,
                AwayTeam = eventItem.AwayTeam,
                AwayWinChance = eventItem.AwayWinChance,
                HomeWinChance = eventItem.HomeWinChance,
                DrawChance = eventItem.DrawChance,
                EventStartDate = eventItem.EventStartDate,
                EventEndDate = eventItem.EventEndDate,
                EventResult = eventItem.EventResult.ToString(),
                Type = eventItem.Type.ToString(),
                EventRounds = relatedRounds
            };

            return Ok(eventDto);
        }

        [HttpPatch("{id}/details")]
        public async Task<IActionResult> UpdateEventDetails([FromRoute] string id, [FromBody] EventDetailUpdateDto detailUpdateDto)
        {
            var events = await _jsonFileService.LoadEventsAsync();
            var eventItem = events.FirstOrDefault(e => e.Id.ToString() == id);

            if (eventItem == null)
            {
                return NotFound($"Event with ID {id} not found.");
            }

            if (eventItem.EventResult != Enums.EventResult.Pending)
            {
                return BadRequest($"Event with Id {id} is Not Active");
            }

            var eventDetails = await _jsonFileService.LoadEventDetailsAsync();
            var eventDetailItem = eventDetails.FirstOrDefault(e => e.EventId.ToString() == id && e.RoundNumber == detailUpdateDto.RoundNumber);

            if (eventDetailItem == null)
            {
                return NotFound($"EventDetail with Round Number {detailUpdateDto.RoundNumber} not found.");
            }

            eventDetailItem.AwayTeamScore = detailUpdateDto.AwayTeamScore ?? eventDetailItem.AwayTeamScore;
            eventDetailItem.HomeTeamScore = detailUpdateDto.HomeTeamScore ?? eventDetailItem.HomeTeamScore;

            await _jsonFileService.SaveEventDetailsAsync(eventDetails);
            await _eventService.UpdateEventsOdds(eventItem);

            return Ok(eventDetailItem);
        }


        [HttpPost("{id}/details/create")]
        public async Task<IActionResult> CreateEventDetail([FromRoute] string id, [FromBody] EventDetailCreateDto detailDto)
        {
            var events = await _jsonFileService.LoadEventsAsync();
            var eventItem = events.FirstOrDefault(e => e.Id.ToString() == id);

            if (eventItem == null)
            {
                return NotFound($"Event with ID {id} not found.");
            }

            if(eventItem.EventResult != Enums.EventResult.Pending)
            {
                return BadRequest($"Event with Id {id} is Not Active");
            }

            var eventDetails = await _jsonFileService.LoadEventDetailsAsync();
            var newDetail = new EventDetail
            {
                Id = Guid.NewGuid(),
                EventId = eventItem.Id,
                RoundNumber = detailDto.RoundNumber ?? 1,
                HomeTeamScore = detailDto.HomeTeamScore ?? 0,
                AwayTeamScore = detailDto.AwayTeamScore ?? 0,
                RoundDateTime = DateTime.UtcNow
            };

            eventDetails.Add(newDetail);
            await _jsonFileService.SaveEventDetailsAsync(eventDetails);
            await _eventService.UpdateEventsOdds(eventItem);

            return CreatedAtAction(nameof(CreateEventDetail), new { id = newDetail.Id }, newDetail);
        }
    }
}
