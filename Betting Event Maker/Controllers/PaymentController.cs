using Betting_Event_Maker.Dtos.Cards;
using Betting_Event_Maker.Models;
using Betting_Event_Maker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Betting_Event_Maker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly JsonFileService _jsonService;
        public PaymentController(JsonFileService jsonService)
        {
            _jsonService = jsonService;
        }

        [HttpPost("registerCard")]
        public IActionResult RegisterCard([FromBody] CardRegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cards = _jsonService.LoadCards();

            var newCard = new PaymentCard
            {
                CardOwnerName = registerDto.CardOwnerName,
                AvailableAmount = registerDto.AvailableAmount ?? 0,
                CardNumber = string.Concat(Enumerable.Range(0, 16).Select(_ => new Random().Next(0, 10).ToString())),
                CVV = new Random().Next(100, 1000).ToString(),
            };

            cards.Add(newCard);
            _jsonService.SaveCards(cards);

            return Ok(newCard);
        }

        [HttpPost("pay")]
        public IActionResult CardPayment([FromBody] CardPaymentDto paymentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cards = _jsonService.LoadCards();
            var card = cards.FirstOrDefault(c => c.CardNumber == paymentDto.CardNumber && c.CardOwnerName == paymentDto.CardOwnerName && c.CVV == paymentDto.CVV);

            if (card == null)
            {
                return NotFound("Invalid Credentials");
            }

            if (card.AvailableAmount < paymentDto.PaymentAmount)
            {
                return BadRequest("Not enough money");
            }

            card.AvailableAmount -= paymentDto.PaymentAmount;
            _jsonService.SaveCards(cards);

            return Ok(new { Message = "Payment Successful", AvailableAmount = card.AvailableAmount, Owner = card.CardOwnerName });
        }

        [HttpPost("addMoney")]
        public IActionResult AddMoney([FromBody] CardAddMoneyDto paymentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var cards = _jsonService.LoadCards();
            var card = cards.FirstOrDefault(c => c.CardNumber == paymentDto.CardNumber);
            if (card == null)
            {
                return NotFound("Invalid Credentials");
            }
            card.AvailableAmount += paymentDto.PaymentAmount;
            _jsonService.SaveCards(cards);
            return Ok(new { Message = "Money Added Successfully", Owner = card.CardOwnerName });
        }

        [HttpGet("info")]
        public IActionResult GetCardNumber([FromBody] CardCredentialsDto credentialsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var cards = _jsonService.LoadCards();
            var card = cards.FirstOrDefault(c => c.CardNumber == credentialsDto.CardNumber && c.CVV == credentialsDto.CVV && c.CardOwnerName == credentialsDto.CardOwnerName);
            if (card == null)
            {
                return NotFound("Invalid Credentials");
            }
            return Ok(new CardInfoDto
            {
                CardOwnerName = card.CardOwnerName,
                AvailableAmount = card.AvailableAmount,
            });
        }
    }
}
