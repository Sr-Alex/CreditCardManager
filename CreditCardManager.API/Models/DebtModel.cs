using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditCardManager.Models
{
    public class DebtModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("CreditCards")]
        public int CardId { get; set; }

        [Required]
        [ForeignKey("Users")]
        public int UserId { get; set; }

        [MaxLength(100)]
        [DefaultValue("User debt")]
        public string Label { get; set; } = "User debt";

        [DataType(DataType.Date)]
        [DefaultValue("GETDATE()")]
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [DataType(DataType.Currency)]
        public decimal Value { get; set; }

        [DefaultValue(false)]
        public bool IsPaid { get; set; } = false;
    }
}