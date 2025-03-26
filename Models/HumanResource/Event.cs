using EnterpriseManagementApp.Enums;
using EnterpriseManagementApp.Models;

public class AppEvent
{
    public int Id { get; set; }

    // Foreign key to Service
    public int ServiceId { get; set; }
    
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    // e.g., "Ongoing", "Completed", etc. You could also use an enum
    public EventStatus Status { get; set; } = 0;
        
    // Navigation properties
    public Service? Service { get; set; }

    // Navigation property for the many-to-many relationship
    public ICollection<Customer> Customers { get; set; } = new List<Customer>();

    // If you track which employees attended this event
    public virtual ICollection<Attendance> Attendances { get; set; }

    public AppEvent()
    {
        Attendances = new HashSet<Attendance>();
    }
}
