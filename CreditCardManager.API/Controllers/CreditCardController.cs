using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using CreditCardManager.DTOs;
using CreditCardManager.Interfaces;

namespace CreditCardManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreditCardController : ControllerBase
    {
        private readonly ICreditCardServices _creditCardServices;
        private readonly ITokenServices _tokenServices;

        public CreditCardController(ICreditCardServices creditCardServices, ITokenServices tokenServices)
        {
            _creditCardServices = creditCardServices;
            _tokenServices = tokenServices;
        }

        [HttpGet]
        public IActionResult GetCreditCards([FromQuery] int userId)
        {
            List<CreditCardDTO> cards = _creditCardServices.GetUserCreditCards(userId);

            if (cards == null || cards.Count == 0)
            {
                return NotFound(new { Message = "No credit cards found" });
            }

            return Ok(cards);
        }

        [HttpGet("details/{id}")]
        public IActionResult GetCreditCard(int id)
        {
            CreditCardDTO? card = _creditCardServices.GetCreditCard(id);

            if (card == null) return NotFound();

            return Ok(card);
        }

        [Authorize]
        [HttpGet("details/{id}/users")]
        public IActionResult GetCreditCardUsers(int id)
        {
            try
            {
                CardUsersDTO result = _creditCardServices.GetUsers(id);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("details/{id}/users")]
        public IActionResult AddUser(int id, [FromBody] int userId)
        {
            string authorization = Request.Headers.Authorization.ToString();
            int userIdToken = _tokenServices.DecodeUserToken(authorization).Id;

            if (!_creditCardServices.IsUserOwnerOfCard(id, userIdToken))
                return Unauthorized(new
                {
                    Message = "You are not authorized to add users to this credit card."
                });

            try
            {
                bool result = _creditCardServices.AddUser(id, userId);
                return result
                    ? Created("Created", new { Message = "User added to credit card successfully." })
                    : Conflict("User already in credit card.");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateCreditCard([FromBody] CreateCreditCardDTO creditCardDTO)
        {
            string authorization = Request.Headers.Authorization.ToString();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                UserDTO userToken = _tokenServices.DecodeUserToken(authorization);
                creditCardDTO.UserId = userToken.Id;

                bool result = _creditCardServices.CreateCreditCard(creditCardDTO);

                return result
                    ? Created("Created", new { Message = "Credit Card created successfully" })
                    : NotFound();
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteCreditCard(int id)
        {
            string authorization = Request.Headers.Authorization.ToString();
            CreditCardDTO? card = _creditCardServices.GetCreditCard(id);

            if (card == null)
                return NotFound();

            try
            {
                int userId = _tokenServices.DecodeUserToken(authorization).Id;
                if (userId != card.UserId)
                    return Unauthorized(new { Message = "You are not authorized to delete this credit card." });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { ex.Message });
            }

            return _creditCardServices.DeleteCreditCard(id)
            ? Ok()
            : NotFound();
        }
    }
}