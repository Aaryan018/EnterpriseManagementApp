namespace EnterpriseManagementApp.Models
{
    public class DashboardViewModel
    {
        // Align with view usage
        public int EmployeeCount { get; set; } // Renamed from TotalEmployees to match view
        public decimal TotalPayroll { get; set; }
        public int ActiveRentals { get; set; }
        public int VacantUnits { get; set; }
        public int PendingServices { get; set; }
        public int CompletedServices { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal AnnualRevenue { get; set; }

        // Additional properties from controller
        public string? UserFullName { get; set; }
        public string? UserEmail { get; set; }
        public int? UpcomingPayments { get; set; } // Changed from object? to int?
        public int? ActiveServices { get; set; } // Changed from object? to int?
        public int LeaseExpiring { get; set; }
        public int ActiveShifts { get; set; }
        public int PendingCertifications { get; set; }
        public int TotalEmployees { get; internal set; }

        // New property for Manager access to employee list
        public bool CanViewEmployeeList { get; set; }
    }
}