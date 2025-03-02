using EnterpriseManagementApp.Enums;

public class LeaveRequest
{
    public int LeaveRequestId { get; set; }

    // Foreign key to Employee
    public int EmployeeId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public LeaveType Type { get; set; }
    
    public bool ApprovalStatus { get; set; }

    // if it is a paid or unpaid leave
    public bool Paid { get; set; }

    public Employee Employee { get; set; }
}
