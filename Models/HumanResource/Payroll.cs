using System;

public class Payroll
{
    public int PayrollId { get; set; }
    // Foreign key to Employee
    public string EmployeeId { get; set; }
    // For Pay Cycle or Month
    public DateTime PayrollGenerate { get; set; }
    public double TotalPay { get; set; }
    public double RegularHours { get; set; }
    public double OvertimeHours { get; set; }
    public double LateTimeHours { get; set; }
    public double HourlyRate { get; set; } // Might come from Employee
    public Employee? Employee { get; set; }
}