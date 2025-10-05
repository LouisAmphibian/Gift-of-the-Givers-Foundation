using System.ComponentModel.DataAnnotations;

namespace Gift_of_the_Givers_Foundation.Models
{
    public class Donation
    {
        public int DonationID { get; set; }

        [Required(ErrorMessage = "Donation type is required")]
        [StringLength(100, ErrorMessage = "Type cannot exceed 100 characters")]
        public string DonationType { get; set; } = string.Empty; // Food, Clothing, Medical, Money, Other

        [Required(ErrorMessage = "Item description is required")]
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [StringLength(50)]
        public string Unit { get; set; } = string.Empty; // kg, pieces, boxes, etc.

        [Required(ErrorMessage = "Location is required")]
        [StringLength(150, ErrorMessage = "Location cannot exceed 150 characters")]
        public string Location { get; set; } = string.Empty;

        [StringLength(20)]
        public string Urgency { get; set; } = string.Empty; // Low, Medium, High, Critical

        [StringLength(500)]
        public string SpecialInstructions { get; set; } = string.Empty;

        public int DonatedByUserID { get; set; }
        public DateTime DonationDate { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Collected, Distributed

        // Navigation property
        public User? DonatedByUser { get; set; }
    }
}