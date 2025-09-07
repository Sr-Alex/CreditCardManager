using System.ComponentModel.DataAnnotations;
using CreditCardManager.Validators;

namespace CreditCardManager.DTOs
{
    public record CreditCardDTO
    {
        public int Id { get; init; }
        public int UserId { get; set; }
        public string CardName { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
        public string Invoice { get; set; } = string.Empty;
        public string Limit { get; init; } = string.Empty;
    }

    public record CreateCreditCardDTO
    {
        public int UserId { get; set; }

        public string CardName { get; set; } = "Credit Card";

        [DataType(DataType.DateTime)]
        [OnlyFutureDate]
        public DateTime ExpiresAt { get; set; } = DateTime.Now.AddDays(30);

        [Range(0, double.MaxValue, ErrorMessage = "Limit must be a positive number.")]
        public decimal Limit { get; set; } = 0.00m;
    }
}