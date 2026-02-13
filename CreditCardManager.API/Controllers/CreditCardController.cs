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
        private readonly ITokenServices _tokenServices;
        private readonly ICreditCardServices _creditCardServices;
        private readonly ICardUserServices _cardUserServices;

        public CreditCardController(ICreditCardServices creditCardServices, ICardUserServices cardUserServices, ITokenServices tokenServices)
        {
            _creditCardServices = creditCardServices;
            _cardUserServices = cardUserServices;
            _tokenServices = tokenServices;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetCreditCards([FromQuery] int userId, [FromHeader] string Authorization)
        {
            int userIdToken = _tokenServices.DecodeUserToken(Authorization).Id;

            try
            {
                List<CreditCardDTO> cards = _creditCardServices.GetUserCreditCards(userIdToken);
                return Ok(cards);
            }
            catch (System.Exception)
            {
                return NotFound("This user does not exist.");
            }

        }

        [HttpGet("details/{cardId}")]
        public IActionResult GetCreditCard(int cardId)
        {
            CreditCardDTO? card = _creditCardServices.GetCreditCard(cardId);

            if (card == null) return NotFound();

            return Ok(card);
        }

        [Authorize]
        [HttpGet("details/{cardId}/users")]
        public IActionResult GetCreditCardUsers(int cardId)
        {
            if (!_creditCardServices.CardIdExists(cardId))
            {
                return NotFound("This credit card does not exist.");
            }

            List<CardUserDTO> result = _cardUserServices.GetCardUsers(cardId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("details/{cardId}/users")]
        public IActionResult AddUser(int cardId, [FromBody] UserEmailDTO userEmailDTO, [FromHeader] string Authorization)
        {
            UserDTO userToken = _tokenServices.DecodeUserToken(Authorization);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!_creditCardServices.IsUserOwnerOfCard(cardId, userToken.Id))
                return Unauthorized(new
                {
                    Message = "You are not authorized to add users to this credit card."
                });

            if (userEmailDTO.UserEmail == userToken.Email)
                return BadRequest(new
                {
                    Message = "You are already the owner of this credit card."
                });

            try
            {
                bool result = _creditCardServices.AddUser(cardId, userEmailDTO.UserEmail);
                return result
                    ? Ok(new { Message = "User added to credit card successfully." })
                    : Conflict("User already in credit card.");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("details/{cardId}/users")]
        public IActionResult RemoveUser(int cardId, [FromBody] DeleteCardUserDTO deleteCardUser, [FromHeader] string Authorization)
        {
            UserDTO userToken = _tokenServices.DecodeUserToken(Authorization);

            if (!ModelState.IsValid) return BadRequest(ModelState);
            Console.WriteLine(deleteCardUser.CardUserId);

            if (!_creditCardServices.IsUserOwnerOfCard(cardId, userToken.Id))
                return Unauthorized(new
                {
                    Message = "You are not authorized to remove users from this credit card.",
                });

            try
            {
                bool result = _creditCardServices.RemoveUser(cardId, deleteCardUser.CardUserId);
                return result
                    ? NoContent()
                    : NotFound("User not linked to this credit card.");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateCreditCard([FromBody] CreateCreditCardDTO creditCardDTO, [FromHeader] string Authorization)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                UserDTO userToken = _tokenServices.DecodeUserToken(Authorization);
                creditCardDTO.UserId = userToken.Id;

                CreditCardDTO card = _creditCardServices.CreateCreditCard(creditCardDTO);

                return Created("Created", card);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteCreditCard(int id, [FromHeader] string Authorization)
        {
            CreditCardDTO? card = _creditCardServices.GetCreditCard(id);

            if (card == null)
                return NotFound();

            try
            {
                int userId = _tokenServices.DecodeUserToken(Authorization).Id;
                if (userId != card.UserId)
                    return Unauthorized(new { Message = "You are not authorized to delete this credit card." });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { ex.Message });
            }

            return _creditCardServices.DeleteCreditCard(id)
            ? NoContent()
            : NotFound();
        }

        public IActionResult GetCreditCards(int id)
        {
            throw new NotImplementedException();
        }

        public IActionResult AddUser(int id, string email, string token)
        {
            throw new NotImplementedException();
        }
    }
}