using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models.Rentals
{
    public class RentHistory
    {
        [Key]
        public int RentHistoryID { get; set; }
        public int RenterID { get; set; }
        public int AssetID { get; set; }
        public decimal RentAmount { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime RentDate { get; set; }
    }
}
