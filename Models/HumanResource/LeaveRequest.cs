using System.ComponentModel.DataAnnotations;
using EnterpriseManagementApp.Enums;

namespace EnterpriseManagementApp.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        // Foreign key to Employee
        [Required]
        public string EmployeeId { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Leave type is required.")]
        public LeaveType Type { get; set; }

        public bool ApprovalStatus { get; set; }

        public bool Paid { get; set; }

        public Employee Employee { get; set; }
    }
}
