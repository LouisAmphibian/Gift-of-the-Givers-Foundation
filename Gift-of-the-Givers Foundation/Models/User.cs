using System.ComponentModel.DataAnnotations;

namespace Gift_of_the_Givers_Foundation.Models
{
    public class User
    {
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber { get; set; } = string.Empty; // Make nullable

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "Donor";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}