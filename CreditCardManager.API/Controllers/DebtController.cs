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
        private readonly IDebtServices _debtServices;
        private readonly ITokenServices _tokenServices;

        public DebtController(IDebtServices debtServices, ITokenServices tokenServices)
        {
            _debtServices = debtServices;
            _tokenServices = tokenServices;
        }

        [HttpGet("{id}")]
        public IActionResult GetDebt(int id)
        {
            DebtDTO? result = _debtServices.GetDebt(id);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateDebt([FromBody] CreateDebtDTO debtDTO, [FromHeader] string Authorization)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                UserDTO user = _tokenServices.DecodeUserToken(Authorization.Substring(7));
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