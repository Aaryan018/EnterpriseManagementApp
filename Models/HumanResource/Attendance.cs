using EnterpriseManagementApp.Enums;

public class Attendance
{
    public int AttendanceId { get; set; }

    // Foreign key to Employee
    public string EmployeeId { get; set; }

    // Foreign key to Event
    public int EventId { get; set; }

    public DateTime ClockedInTime { get; set; }
    public DateTime ClockedOutTime { get; set; }
    public DayType DayType { get; set; }

    // Navigation properties
    public Employee Employee { get; set; }
    public Event Event { get; set; }
}
