using EnterpriseManagementApp.Enums;

namespace EnterpriseManagementApp.Models;

public class LeaveRequest
{
    public int Id { get; set; }

    // Foreign key to Employee
    public string EmployeeId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public LeaveType Type { get; set; }
    
    public bool ApprovalStatus { get; set; }

    // if it is a paid or unpaid leave
    public bool Paid { get; set; }

    public Employee? Employee { get; set; }
}
