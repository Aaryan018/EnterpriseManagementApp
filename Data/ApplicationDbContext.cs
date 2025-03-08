using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EnterpriseManagementApp.Models;
using EnterpriseManagementApp.Models.Employees;
using EnterpriseManagementApp.Models.Rentals;
using EnterpriseManagementApp.Models.Services;
using EnterpriseManagementApp.Models.Certifications;

namespace EnterpriseManagementApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Renter> Renters { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<PayrollDetails> PayrollDetails { get; set; }
        public DbSet<DamageReport> DamageReports { get; set; }
        public DbSet<LeaseAgreement> LeaseAgreements { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<EmployeeService> EmployeeServices { get; set; }
        public DbSet<ServiceSchedule> ServiceSchedules { get; set; }
        public DbSet<EmployeeCertifications> EmployeeCertifications { get; set; }
        public DbSet<RentHistory> RentHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Profile Relationship (One-to-One with ApplicationUser)
            modelBuilder.Entity<Profile>()
                .HasOne(p => p.ApplicationUser)
                .WithOne(u => u.Profile)
                .HasForeignKey<Profile>(p => p.ApplicationUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Employee-Service Many-to-Many Relationship
            modelBuilder.Entity<EmployeeService>()
                .HasKey(es => new { es.EmployeeID, es.ServiceID });

            modelBuilder.Entity<EmployeeService>()
                .HasOne(es => es.Employee)
                .WithMany(e => e.EmployeeServices)
                .HasForeignKey(es => es.EmployeeID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmployeeService>()
                .HasOne(es => es.Service)
                .WithMany(s => s.EmployeeServices)
                .HasForeignKey(es => es.ServiceID)
                .OnDelete(DeleteBehavior.Restrict);

            // ServiceSchedule Relationships
            modelBuilder.Entity<ServiceSchedule>()
                .HasOne(ss => ss.Client)
                .WithMany(c => c.ServiceSchedules)
                .HasForeignKey(ss => ss.ClientID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServiceSchedule>()
                .HasOne(ss => ss.Service)
                .WithMany(s => s.ServiceSchedules)
                .HasForeignKey(ss => ss.ServiceID)
                .OnDelete(DeleteBehavior.Restrict);

            // PayrollDetails Relationship
            modelBuilder.Entity<PayrollDetails>()
                .HasOne(pd => pd.Payroll)
                .WithMany(p => p.PayrollDetails)
                .HasForeignKey(pd => pd.PayrollID)
                .OnDelete(DeleteBehavior.Cascade);

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

            modelBuilder.Entity<Invoice>()
                .Property(i => i.Amount)
                .HasColumnType("decimal(18,2)");
        }
    }
}
