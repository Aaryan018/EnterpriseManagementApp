using System;
using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models.Rentals
{
    public class Renter
    {
        [Key]
        public int RenterID { get; set; }

        [Required]  // Ensures Name is not null
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string EmergencyContact { get; set; } = string.Empty;

        [Required]  // OccupancyStartDate should always have a value
        public DateTime OccupancyStartDate { get; set; }

        public DateTime? OccupancyEndDate { get; set; } // Nullable for flexibility

        // Computed property (Not stored in DB)
        public string FullName => Name;
    }
}
