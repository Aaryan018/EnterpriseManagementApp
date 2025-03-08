using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnterpriseManagementApp.Models.Employees
{
    public class Attendance
    {
        [Key]
        public int AttendanceID { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeID { get; set; }
        public Employee? Employee { get; set; }

        public DateTime Date { get; set; }

        [Required, MaxLength(20)]
        public string? Status { get; set; }  // Present, Absent
    }
}


