using System;

public class Payroll
{
    public int PayrollId { get; set; }

    // Foreign key to Employee
    public string EmployeeId { get; set; }

    // For Pay Cycle or Month; 
    public DateTime PayrollGenerate { get; set; }

    public double TotalPay { get; set; }

    public double RegularHours { get; set; }
    public double OvertimeHours { get; set; }
    public double LateTimeHours { get; set; }

    // Navigation property
    public Employee Employee { get; set; }

    // Regular shift end time (5 PM)
    private readonly DateTime _regularShiftEndTime = new DateTime(1, 1, 1, 17, 0, 0); // 5:00 PM

    // Method to calculate overtime hours based on ClockedOutTime
    public double CalculateOvertime(Attendance attendance)
    {
        double overtimeHours = 0;

        // Check if the employee's clock-out time is after 5 PM
        if (attendance.ClockedOutTime > _regularShiftEndTime)
        {
            // Calculate overtime as the difference between clock-out time and 5 PM
            overtimeHours = (attendance.ClockedOutTime - _regularShiftEndTime).TotalHours;
        }

        return overtimeHours;
    }

    // Assuming the standard shift starts at 9 AM
    private readonly DateTime _standardStartTime = new DateTime(1, 1, 1, 9, 0, 0);

    // Method to calculate late time deduction based on clock-in time
    public double CalculateLateTimeDeduction(Attendance attendance, Employee employee)
    {
        double lateTimeHours = 0;

        // Check if the employee clocked in late (after 9 AM)
        if (attendance.ClockedInTime > _standardStartTime)
        {
            // Calculate the difference between the clock-in time and 9 AM
            lateTimeHours = (attendance.ClockedInTime - _standardStartTime).TotalHours;
        }

        // Assuming the late time deduction is half of the hourly rate per hour of late time (you can change this logic)
        double lateTimeDeduction = (double)(lateTimeHours * (employee.HourlyRate * 0.5)); // 50% of hourly rate per hour late

        return lateTimeDeduction;
    }

    public double CalculateTotalPay(Employee employee, double regularHours, double overtimeHours, double lateTimeDeduction)
    {

        // Fetch HourlyRate from Employee
        double hourlyRate = (double)Employee.HourlyRate;

        // Example formula: TotalPay = RegularPay + OvertimePay (Overtime pay rate could be a multiplier, e.g., 1.5x)
        double regularPay = RegularHours * hourlyRate;
        double overtimePay = overtimeHours * (hourlyRate * 1.5); // Assuming overtime is paid at 1.5x the regular rate

        double TotalPay = regularPay + overtimePay - lateTimeDeduction;
        return TotalPay;
    }

    // Validation method for ensuring no negative hours (optional)
    public bool IsValidPayroll()
    {
        if (RegularHours < 0 || OvertimeHours < 0 || LateTimeHours < 0)
        {
            return false; // Invalid if any hours are negative
        }
        return true;
    }
}

