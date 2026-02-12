using System.ComponentModel.DataAnnotations;

namespace CreditCardManager.DTOs
{
    public record CardUserDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string UserName { get; set; }
        public int DebtsCount { get; set; }
        public int PendingDebts { get; set; }
    }

    public record CreateCardUserDTO
    {
        [Required(ErrorMessage = "The credit card id is required.")]
        public required int CardId { get; set; }
        [Required(ErrorMessage = "The user id is required.")]
        public required int UserId { get; set; }
    }

    public record DeleteCardUserDTO
    {
        [Required(ErrorMessage = "The card user id is required.")]
        public required int CardUserId { get; set; }
    }
}