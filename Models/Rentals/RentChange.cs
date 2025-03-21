using System;
using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models
{
    public class RentChange
    {
        [Key]
        public Guid RentChangeId { get; set; }

        // Foreign keys (required)
        [Required]
        public Guid AssetId { get; set; } // Foreign key for Asset
        [Required]
        public string? UserId { get; set; } // Foreign key for ApplicationUser

        // Navigation properties
        public Asset? Asset { get; set; }
        public ApplicationUser User { get; set; }

        [Required(ErrorMessage = "ChangeDate is required.")]
        public DateTime ChangeDate { get; set; }

        [Required(ErrorMessage = "Old Rate is required.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$",
            ErrorMessage = "Old Rate must be a valid amount in the format of '123.45' with up to two decimal places.")]
        public double OldRate { get; set; }

        [Required(ErrorMessage = "New Rate is required.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$",
            ErrorMessage = "New Rate must be a valid amount in the format of '123.45' with up to two decimal places.")]
        public double NewRate { get; set; }

        [Required(ErrorMessage = "Reason is required.")]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
        public string Reason { get; set; }

        [Required]
        public string Status { get; set; } = "Pending"; // Possible values: Pending, Approved, Rejected

        public DateTime SubmittedDate { get; set; } = DateTime.Now;
        public DateTime? ProcessedDate { get; set; }
    }
}