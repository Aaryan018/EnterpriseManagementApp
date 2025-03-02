public class Service
{
    public int ServiceId { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    
    // Example: duration in minutes or hours; choose what makes sense
    public int Duration { get; set; }  
    
    // Could be decimal if you need currency precision
    public double Price { get; set; }
    
    // If the service requires specific qualifications (e.g., "License A")
    public string Qualifications { get; set; }

    // Navigation property: one service can have many events
    public virtual ICollection<Event> Events { get; set; }

    public Service()
    {
        Events = new HashSet<Event>();
    }
}
