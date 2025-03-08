using EnterpriseManagementApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnterpriseManagementApp.Models.Employees
{
    public class Shift
    {
        [Key]
        public int ShiftID { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeID { get; set; }
        public Employee? Employee { get; set; }

        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
}
