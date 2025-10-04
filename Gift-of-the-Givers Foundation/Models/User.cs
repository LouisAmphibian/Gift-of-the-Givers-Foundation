using System.ComponentModel.DataAnnotations;

namespace Gift_of_the_Givers_Foundation.Models
{
    public class User
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string PasswordHash { get; set; }

        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [StringLength(50, ErrorMessage = "Role cannot exceed 50 characters")]
        [RegularExpression("^(Donor|Admin|Volunteer)$", ErrorMessage = "Role must be Donor, Admin, or Volunteer")]
        public string Role { get; set; } = "Donor";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}