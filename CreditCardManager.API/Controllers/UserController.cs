using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using CreditCardManager.DTOs;
using CreditCardManager.Interfaces;


namespace CreditCardManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly ITokenServices _tokenServices;

        public UserController(IUserServices userServices, ITokenServices tokenServices)
        {
            _userServices = userServices;
            _tokenServices = tokenServices;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetUsers()
        {
            List<UserDTO> users = _userServices.GetUsers();

            if (users == null || users.Count == 0)
            {
                return NotFound(new { Message = "No users found" });
            }

            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetUser([FromRoute] int id)
        {
            UserDTO? user = _userServices.GetUser(id);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            return Ok(user);
        }


        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                UserDTO user = _userServices.Create(userDTO);

                return Created("Created", new
                {
                    Message = "User created successfully",
                    token = _tokenServices.GenerateUserToken(user)
                });
            }
            catch (Exception ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }

        [HttpPost("Login")]
        public IActionResult LoginUser([FromBody] LoginUserDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                UserDTO result = _userServices.Login(loginDTO);
                string token = _tokenServices.GenerateUserToken(result);

                return Ok(new { token });
            }
            catch (Exception)
            {
                return Unauthorized(new { Message = "Invalid email or password" });
            }
        }
    }

}