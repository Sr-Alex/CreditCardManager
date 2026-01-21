using System.ComponentModel.DataAnnotations;

namespace CreditCardManager.DTOs
{
    public record CardUserDTO
    {
        public int UserId { get; set; }
        public required string UserName { get; set; }
        public int DebtsCount { get; set; }
        public int PendingDebts { get; set; }
    }

    public record CreateCardUserDTO
    {
        [Required(ErrorMessage = "The credit card id is required.")]
        public required int CardId;

        [Required(ErrorMessage = "The user id is required.")]
        public required int UserId;
    }
}