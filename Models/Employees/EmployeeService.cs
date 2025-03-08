// Models/Employees/EmployeeService.cs
using EnterpriseManagementApp.Models.Services;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnterpriseManagementApp.Models.Employees
{
    public class EmployeeService
    {
        [ForeignKey(nameof(Employee))]
        public int EmployeeID { get; set; }

        [ForeignKey(nameof(Service))]
        public int ServiceID { get; set; }

        // Navigation properties
        public Employee Employee { get; set; } = null!;
        public Service Service { get; set; } = null!;
    }
}