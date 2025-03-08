using EnterpriseManagementApp.Models.Employees;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EnterpriseManagementApp.Models.Services
{
    public class Service
    {
        [Key]
        public int ServiceID { get; set; }

        [Required, MaxLength(100)]
        public string? Name { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Rate { get; set; }

        // Foreign Key to Client
        public int? ClientID { get; set; }
        public Client? Client { get; set; }

        // Navigation property for many-to-many relationship with Employee
        public ICollection<EmployeeService> EmployeeServices { get; set; } = new List<EmployeeService>();

        // Navigation property for associated ServiceSchedules (One-to-many)
        public ICollection<ServiceSchedule> ServiceSchedules { get; set; } = new List<ServiceSchedule>();
    }
}