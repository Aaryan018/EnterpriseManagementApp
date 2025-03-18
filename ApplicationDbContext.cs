namespace EnterpriseManagementApp
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using EnterpriseManagementApp.Models;

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
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<OccupancyHistory> OccupancyHistories { get; set; }
        public DbSet<RentChange> RentChanges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .ToTable("Employees");

            modelBuilder.Entity<Customer>()
                .ToTable("Customers");

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Customers)
                .WithMany(c => c.Events);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Service)
                .WithMany(s => s.Events)
                .HasForeignKey(e => e.ServiceId);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Event)
                .WithMany(e => e.Attendances)
                .HasForeignKey(a => a.EventId);

            modelBuilder.Entity<OccupancyHistory>()
                .HasKey(oh => new { oh.CustomerId, oh.AssetId });

            modelBuilder.Entity<OccupancyHistory>()
                .HasOne(oh => oh.Customer)
                .WithMany(r => r.OccupancyHistories)
                .HasForeignKey(oh => oh.CustomerId);

            modelBuilder.Entity<OccupancyHistory>()
                .HasOne(oh => oh.Asset)
                .WithMany(a => a.OccupancyHistories)
                .HasForeignKey(oh => oh.AssetId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RentChange>()
                .HasOne(rc => rc.Asset)
                .WithMany(a => a.RentChanges)
                .HasForeignKey(rc => rc.AssetId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            modelBuilder.Entity<RentChange>()
                .HasOne(rc => rc.User)
                .WithMany()
                .HasForeignKey(rc => rc.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}