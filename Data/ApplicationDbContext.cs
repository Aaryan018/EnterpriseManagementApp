using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EnterpriseManagementApp.Models;
using EnterpriseManagementApp.Models.Rentals;

namespace EnterpriseManagementApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Event> Events { get; set; }


        public DbSet<Renter> Renters { get; set; }

        public DbSet<DamageReport> DamageReports { get; set; }
        public DbSet<LeaseAgreement> LeaseAgreements { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<RentHistory> RentHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .ToTable("Employees")
                .HasKey(e => e.EmployeeId);

             // Profile Relationship (One-to-One with ApplicationUser)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.ApplicationUser)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.ApplicationUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Example of linking Event -> Service
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Service)
                .WithMany(s => s.Events)
                .HasForeignKey(e => e.ServiceId);

            // Attendance -> Event
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Event)
                .WithMany(e => e.Attendances)
                .HasForeignKey(a => a.EventId);

            // Profile Relationship (One-to-One with ApplicationUser)
            modelBuilder.Entity<Profile>()
                .HasOne(p => p.ApplicationUser)
                .WithOne(u => u.Profile)
                .HasForeignKey<Profile>(p => p.ApplicationUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Renter Configuration
            modelBuilder.Entity<Renter>()
                .Property(r => r.OccupancyStartDate)
                .IsRequired(); // Ensure NOT NULL

            modelBuilder.Entity<Renter>()
                .Property(r => r.OccupancyEndDate)
                .IsRequired(false); // Allow NULL

            // Specify precision and scale for decimal properties to address warnings
            modelBuilder.Entity<LeaseAgreement>()
                .Property(l => l.RentAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<RentHistory>()
                .Property(rh => rh.RentAmount)
                .HasColumnType("decimal(18,2)");

           

        }
    }
}
