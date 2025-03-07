using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnterpriseManagementApp.Models
{
    public class OccupancyHistory
    {
        [Key]
        public Guid OccupancyId { get; set; }
        public int RenterId { get; set; }

        public Guid CustomerId { get; set; }

        [Required]
        public string AssetId { get; set; } // FK to Asset

        public DateOnly Start { get; set; }
        public DateOnly? End { get; set; }
        public double AmountDue { get; set; }
        public bool Status { get; set; } // Approved, Pending

        // Navigation Properties
        [ForeignKey("AssetId")]  // Explicitly map AssetId as a foreign key to Asset
        public virtual Asset Asset { get; set; }

        [ForeignKey("CustomerId")]  // Explicitly map CustomerId as a foreign key to Renter
        public virtual Renter Customer { get; set; }
    }
}
