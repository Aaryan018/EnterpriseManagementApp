using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models
{
    public class Asset
    {
        [Key]
        public Guid AssetId { get; set; }

        //// ___________ Relationship Based Attributes ___________
        //// 1-to-many relationship with Customer (an asset can only have one current renter)
        //public Guid? CustomerId { get; set; }  // This is for the 1-to-many relationship
        //public Customer? Customer { get; set; }  // Navigation property to the current customer


        // many-to-many relationship with Renter (via jointable : OccupancyHistory)
        public ICollection<OccupancyHistory>? OccupancyHistories { get; set; }  // Historical renting information


        // 1-to-many relationship with RentChange
        public ICollection<RentChange>? RentChanges { get; set; }  // An asset can have many rent changes


        // _______________ Own Table Attributes _______________
        [Required(ErrorMessage = "Asset name is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Asset name must be between 1 and 50 characters long.")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Rent Rate is required.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$",
            ErrorMessage = "Rent rate must be a valid amount in the format of '123.45' with up to two decimal places.")]
        public double RentRate { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Description must be between 1 and 200 characters long.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        // I.e. Parking Lot, Suite, Locker, etc
        public string Type { get; set; }

        // Manager approved, or still waiting for review. If Manager disapproves, delete record
        public bool Status { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters long.")]
        public string Address { get; set; }

        public DateOnly? CreatedAt { get; set; }

        public DateOnly? UpdatedAt { get; set; }
    }
}
