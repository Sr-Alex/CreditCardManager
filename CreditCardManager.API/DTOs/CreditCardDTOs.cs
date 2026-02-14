using System.ComponentModel.DataAnnotations;
using CreditCardManager.Validators;

namespace CreditCardManager.DTOs
{
    public record CreditCardDTO
    {
        public int Id { get; init; }
        public int UserId { get; set; }
        public required string CardName { get; init; }
        public DateOnly ExpiresAt { get; init; }
        public required decimal Invoice { get; set; }
        public required decimal Limit { get; set; }
        public int PendantDebts { get; set; }
    }

    public record CreateCreditCardDTO
    {
        public int UserId { get; set; }

        public string CardName { get; set; } = "Credit Card";

        [DataType(DataType.Date)]
        [OnlyFutureDate]
        public DateOnly ExpiresAt { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddDays(30));

        [Range(0, double.MaxValue, ErrorMessage = "Limit must be a positive number.")]
        public decimal Limit { get; set; } = 0.00m;
    }
}