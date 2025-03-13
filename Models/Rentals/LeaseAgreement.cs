using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models.Rentals;


public class LeaseAgreement
{
    [Key]
    public int LeaseID { get; set; }
    public int RenterID { get; set; }
    public int AssetID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal RentAmount { get; set; }
}
