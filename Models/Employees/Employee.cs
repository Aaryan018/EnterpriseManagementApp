using EnterpriseManagementApp.Models.Employees;
using EnterpriseManagementApp.Models.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnterpriseManagementApp.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;  // Default value to avoid null
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? EmergencyContact { get; set; }

        [Required, MaxLength(50)]
        public string JobTitle { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string EmploymentType { get; set; } = "Full Time";

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Salary { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? HourlyRate { get; set; }

        [ForeignKey("Manager")]
        public int? ReportsTo { get; set; }  // Nullable manager reference

        [Required, MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }

        public DateTime HireDate { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Active";

        // **Navigation Properties**
        public Employee? Manager { get; set; }

        public ICollection<EmployeeService> EmployeeServices { get; set; } = new List<EmployeeService>();

        // **Role-based Access**
        [Required, MaxLength(20)]
        public string Role { get; set; } = "User";

        // **Link to Identity System**
        [ForeignKey("UserId")]
        public string? UserId { get; set; }  // Link to ApplicationUser (string, GUID)

        public ApplicationUser? User { get; set; }

        // **Computed Role Properties** (Fixed IsUser)
        public bool IsAdmin => Role == "Admin";
        public bool IsManager => Role == "Manager";
        public bool IsUser => Role == "User" || (!IsAdmin && !IsManager); // Corrected to identify non-admin, non-manager users

        public ICollection<Shift>? Shifts { get; set; }

        // **Access Control Method** (Updated for consistency with UserId)
        public bool CanView(Employee targetEmployee)
        {
            if (this.IsAdmin)
            {
                return true; // Admins can view all employees
            }

            if (this.IsManager)
            {
                return targetEmployee.ReportsTo == this.EmployeeID; // Managers can view direct reports
            }

            // Employees can view only their own data, using UserId if available
            return this.EmployeeID == targetEmployee.EmployeeID ||
                   (this.UserId != null && this.UserId == targetEmployee.UserId);
        }
    }
}