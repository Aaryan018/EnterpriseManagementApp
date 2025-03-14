public class Payroll
{
    public int Id { get; set; }

    // Foreign key to Employee
    public string EmployeeId { get; set; }

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
