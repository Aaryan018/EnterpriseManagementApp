using Microsoft.EntityFrameworkCore;

namespace EnterpriseManagementApp
{
    public class ClientContext : DbContext
    {
        public ClientContext(DbContextOptions<ClientContext> options) : base(options) { }
        public ClientContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .ToTable("People"); 

            modelBuilder.Entity<Employee>()
                .ToTable("Employees");

            // In TPT, the derived table’s primary key also references
            // the base table's primary key (PersonId).
            modelBuilder.Entity<Employee>()
                .Property(e => e.PersonId)
                .ValueGeneratedNever();

            // Self-referencing manager relationship
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany(m => m.Subordinates)
                .HasForeignKey(e => e.ManagerId);


            // Example of linking Event -> Service
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Service)
                .WithMany(s => s.Events)
                .HasForeignKey(e => e.ServiceId);

            // // Example of linking Event -> Person (as Customer)
            // modelBuilder.Entity<Event>()
            //     .HasOne(e => e.Customer)
            //     .WithMany() // or with a "CustomerEvents" collection if you want
            //     .HasForeignKey(e => e.CustomerId);

            // Attendance -> Event
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Event)
                .WithMany(e => e.Attendances)
                .HasForeignKey(a => a.EventId);

        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Event> Events { get; set; }

    }
}