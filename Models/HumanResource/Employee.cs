using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EnterpriseManagementApp.Enums;
using EnterpriseManagementApp.Models;

public class Employee : ApplicationUser
{
    public string? JobTitle { get; set; }
    public double? HourlyRate { get; set; }
    public EmployeeType? Type {get; set;}    
    public string? Qualifications { get; set; }

    // Navigation to related tables
    public ICollection<Attendance> Attendances { get; set; }
    public ICollection<Payroll> Payrolls { get; set; }
    public ICollection<LeaveRequest> LeaveRequests { get; set; }

    public Employee()
    {
        Attendances = new HashSet<Attendance>();
        Payrolls = new HashSet<Payroll>();
        LeaveRequests = new HashSet<LeaveRequest>();

    }
}