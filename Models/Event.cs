using EnterpriseManagementApp.Enums;

public class Event
{
    public int EventId { get; set; }

    // Foreign key to Service
    public int ServiceId { get; set; }

    // Foreign key to Person (assuming "Customer" is a Person)
    //public int CustomerId { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    // e.g., "Ongoing", "Completed", etc. You could also use an enum
    public EventStatus Status { get; set; }

    // Navigation properties
    public Service Service { get; set; }
    //public Person Customer { get; set; }

    // If you track which employees attended this event
    public virtual ICollection<Attendance> Attendances { get; set; }

    public Event()
    {
        Attendances = new HashSet<Attendance>();
    }
}
