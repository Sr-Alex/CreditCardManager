using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditCardManager.Models
{
    public class CreditCardModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Users")]
        public int UserId { get; set; }

        [MaxLength(100)]
        [DefaultValue("Credit Card")]
        public string CardName { get; set; } = "Credit Card";

        [DataType(DataType.DateTime)]
        [DefaultValue("GETDATE()")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [DataType(DataType.DateTime)]
        public DateTime ExpiresAt { get; set; } = DateTime.Now.AddDays(30);

        [DefaultValue(0.00)]
        [DataType(DataType.Currency)]
        public decimal Limit { get; set; } = 0.00m;

        [DefaultValue(0.00)]
        [DataType(DataType.Currency)]
        public decimal Invoice { get; set; } = 0.00m;
    }

}