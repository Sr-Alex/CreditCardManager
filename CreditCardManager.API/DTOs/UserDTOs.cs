using System.ComponentModel.DataAnnotations;

namespace CreditCardManager.DTOs
{
    public record UserDTO
    {
        public int Id { get; init; }
        public string UserName { get; init; } = default!;
        public string Email { get; init; } = default!;
    }

    public record class CreateUserDTO
    {
        [Required(ErrorMessage = "UserName is required.")]
        public string UserName { get; set; } = default!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; } = default!;
    }

    public record LoginUserDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
    }
}