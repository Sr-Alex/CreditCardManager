using System.ComponentModel.DataAnnotations;
using CreditCardManager.Validators;

namespace CreditCardManager.DTOs
{
    public record DebtDTO
    {
        public required int User { get; set; }
        public required int Card { get; set; }

        public string Label { get; set; } = default!;
        public decimal Value;
        public DateTime Date;
    }

    public record CreateDebtDTO
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "CardId is required.")]
        public int CardId { get; set; }

        public string Label { get; set; } = "User debt";

        [DataType(DataType.DateTime)]
        [OnlyPastDate]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Value is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "The Value must be greater than 0.")]
        public decimal Value { get; set; }
    }
}