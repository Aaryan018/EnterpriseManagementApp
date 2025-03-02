public class Payroll
{
    public int PayrollId { get; set; }

    // Foreign key to Employee
    public int EmployeeId { get; set; }

    // For Pay Cycle or Month; 
    public DateTime MonthDate { get; set; }

    public double TotalPay { get; set; }

    public double RegularHours { get; set; }
    public double OvertimeHours { get; set; }
    public double LateTimeHours { get; set; }
    public double HourlyRate { get; set; }   // might also come from Employee

    // Navigation property
    public Employee Employee { get; set; }
}
