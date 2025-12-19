using CreditCardManager.DTOs;
using CreditCardManager.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            DebtDTO? result = _debtServices.GetDebt(id);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetCardDebts([FromQuery] int cardId, [FromHeader] string Authorization)
        {
            try
            {
                int userId = _tokenServices.DecodeUserToken(Authorization).Id;

                if (!_creditCardServices.IsUserOwnerOfCard(cardId, userId)) return Unauthorized();
            }
            catch (System.Exception e)
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
    }
}