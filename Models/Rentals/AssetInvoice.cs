using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models.Rentals
{
    public class AssetInvoice
    {

        [Key]
        public Guid AssetInvoiceId { get; set; }


        // ___________ Relationship Based Attributes ___________
        public String CustomerId { get; set; }  // Foreign key to Customer
        public Guid AssetId { get; set; }

        // Navigation property to OccupancyHistory
        public OccupancyHistory? OccupancyHistory { get; set; }


        // _______________ Own Table Attributes _______________

        public DateTime DatePaid { get; set; }

        public double AmmountPaid { get; set; }
    }
}
