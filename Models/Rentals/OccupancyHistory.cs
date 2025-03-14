using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models
{
    public class OccupancyHistory
    {
        [Key]
        public Guid OccupancyHistoryId { get; set; }

        // ___________ Relationship Based Attributes ___________
        //public Guid CustomerId { get; set; }
        public string CustomerId { get; set; }  // Foreign Key to Customer (using string to match IdentityUser)
        public Customer? Customer { get; set; }

        public Guid AssetId { get; set; }
        public Asset? Asset { get; set; }


        // _______________ Own Table Attributes _______________

        // Data type may change in future
        public DateOnly Start { get; set; }


        // Data type may change in future
        public DateOnly End { get; set; }

        public double Paid { get; set; }

        public double AmmountDue { get; set; }

        // Approved, Pending, Denied
        public string? Status { get; set; }
    }
}
