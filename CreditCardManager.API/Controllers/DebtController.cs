using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using CreditCardManager.DTOs;
using CreditCardManager.Interfaces;

namespace CreditCardManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DebtController : ControllerBase
    {
        private readonly ITokenServices _tokenServices;
        private readonly IDebtServices _debtServices;
        private readonly ICreditCardServices _creditCardServices;

        public DebtController(ITokenServices tokenServices, IDebtServices debtServices, ICreditCardServices creditCardServices)
        {
            _tokenServices = tokenServices;
            _debtServices = debtServices;
            _creditCardServices = creditCardServices;
        }

        [HttpGet("{id}")]
        public IActionResult GetDebt(int id)
        {
            DebtDTO? debt = _debtServices.GetDebt(id);

            if (debt == null) return NotFound();

            return Ok(debt);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetCardDebts([FromQuery] int cardId, [FromHeader] string Authorization)
        {
            try
            {
                int userId = _tokenServices.DecodeUserToken(Authorization).Id;

                if (!_creditCardServices.IsCardUser(cardId, userId)) throw new Exception("User does not have access to this card's debts.");
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }

            return Ok(_debtServices.GetCardDebts(cardId));
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateDebt([FromBody] CreateDebtDTO debtDTO, [FromHeader] string Authorization)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                UserDTO user = _tokenServices.DecodeUserToken(Authorization);
                debtDTO.UserId = user.Id;
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }

            try
            {
                _debtServices.CreateDebt(debtDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Created("Created", new { Message = "User debt created successfully" });
        }

        [Authorize]
        [HttpPut("debts/{id}")]
        public IActionResult UpdateDebt(int id, [FromBody] UpdateDebtDTO debtData, [FromHeader] string Authorization)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DebtDTO? debt = _debtServices.GetDebt(id);
            if (debt == null)
            {
                return NotFound();
            }

            int userId = _tokenServices.DecodeUserToken(Authorization).Id;
            if (!_creditCardServices.IsUserOwnerOfCard(debt.Card, userId))
            {
                return Unauthorized(new { Message = "You are not authorized to update this debt." });
            }

            DebtDTO result = _debtServices.UpdateDebt(id, debtData);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("debts/{id}")]
        public IActionResult DeleteDebt(int id, [FromHeader] string Authorization)
        {
            DebtDTO? debt = _debtServices.GetDebt(id);
            if (debt == null)
            {
                return NotFound(new { Message = "Debt not found." });
            }

            int userId = _tokenServices.DecodeUserToken(Authorization).Id;
            if (!_creditCardServices.IsUserOwnerOfCard(debt.Card, userId))
            {
                return Unauthorized(new { Message = "You are not authorized to delete this debt." });
            }

            bool result = _debtServices.DeleteDebt(id);
            return result ? Ok(new { Message = "Debt deleted successfully." }) : BadRequest(new { Message = "Failed to delete debt." });
        }
    }
}