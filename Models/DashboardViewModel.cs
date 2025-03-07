namespace EnterpriseManagementApp.Models
{
    public class DashboardViewModel
    {
        public int TotalAssets { get; set; }
        public int TotalRenters { get; set; }
        public int ActiveOccupancies { get; set; }
        public int PendingRentChanges { get; set; }
    }
}
