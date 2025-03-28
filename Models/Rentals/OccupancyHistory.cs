using System.ComponentModel.DataAnnotations;
using EnterpriseManagementApp.Models.Rentals;

namespace EnterpriseManagementApp.Models
{
    public class OccupancyHistory
    {
        [Key]
        public Guid OccupancyHistoryId { get; set; } = Guid.NewGuid();

        // ___________ Relationship Based Attributes ___________
        public string CustomerId { get; set; }  // Foreign Key to Customer (using string to match IdentityUser)
        public Customer? Customer { get; set; }

        public Guid AssetId { get; set; }
        public Asset? Asset { get; set; }


        // Navigation property for the related AssetInvoices
        public List<AssetInvoice> AssetInvoices { get; set; } = new List<AssetInvoice>();


        // _______________ Own Table Attributes _______________

        // Data type may change in future
        public DateOnly Start { get; set; }


        // Data type may change in future
        public DateOnly End { get; set; }

        public double Paid { get; set; }

        public double TotalDue { get; set; }

        public double RemainingBalance { get; set; } = 0.0;

        // Approved, Pending, Denied
        public string Status { get; set; } = "Pending";
    }
}
