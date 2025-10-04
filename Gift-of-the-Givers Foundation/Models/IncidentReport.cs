using System.ComponentModel.DataAnnotations;

namespace Gift_of_the_Givers_Foundation.Models
{
    public class IncidentReport
    {
        public int IncidentID { get; set; }

        [Required(ErrorMessage = "Incident title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please describe the incident")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        [StringLength(150, ErrorMessage = "Location cannot exceed 150 characters")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select incident type")]
        [StringLength(100, ErrorMessage = "Incident type cannot exceed 100 characters")]
        public string IncidentType { get; set; } = string.Empty; // Flood, Fire, Earthquake, etc.

        [Required(ErrorMessage = "Please select severity level")]
        [StringLength(50, ErrorMessage = "Severity cannot exceed 50 characters")]
        public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical

        public int ReportedByUserID { get; set; } // Link to the user who reported it

        public DateTime ReportedDate { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string Status { get; set; } = "Reported"; // Reported, Under Review, Resolved

        // Navigation property
        public User? ReportedByUser { get; set; }
    }
}