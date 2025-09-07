using System.ComponentModel.DataAnnotations;

namespace CreditCardManager.DTOs
{
    public record CardUsersDTO
    {
        public int CardId;
        public List<UserDTO> Users = [];
    }

    public record CreateCardUserDTO
    {
        [Required(ErrorMessage = "The credit card id is required.")]
        public required int CardId;

        [Required(ErrorMessage = "The user id is required.")]
        public required int UserId;
    }
}