using System.ComponentModel.DataAnnotations;
using CreditCardManager.Validators;

namespace CreditCardManager.DTOs
{
    public record DebtDTO
    {
        public int Id { get; set; }
        public int User { get; set; }
        public int Card { get; set; }

        public string Label { get; set; } = default!;
        public decimal Value;
        public DateTime Date;
    }

    public record CreateDebtDTO
    {
        [Required(ErrorMessage = "UserId is required.")]
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
    public record UpdateDebtDTO
    {
        public string? Label { get; set; }

        [OnlyPastDate]
        public DateTime? Date { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "The Value must be greater than 0.")]
        public decimal? Value { get; set; }
    }
}