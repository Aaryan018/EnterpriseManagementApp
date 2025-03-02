using EnterpriseManagementApp.Enums;

public class Employee : Person
{
    public string JobTitle { get; set; }
    public double HourlyRate { get; set; }
    public EmployeeType Type {get; set;}

    // Self-reference for Manager
    public int? ManagerId { get; set; }
    
    public string Qualifications { get; set; }

    // Navigation to related tables
    public ICollection<Attendance> Attendances { get; set; }
    public ICollection<Payroll> Payrolls { get; set; }
    public ICollection<LeaveRequest> LeaveRequests { get; set; }
    public ICollection<Employee> Subordinates { get; set; }
    public Employee Manager { get; set; }

    public Employee()
    {
        Attendances = new HashSet<Attendance>();
        Payrolls = new HashSet<Payroll>();
        LeaveRequests = new HashSet<LeaveRequest>();
        Subordinates = new HashSet<Employee>();

    }
}