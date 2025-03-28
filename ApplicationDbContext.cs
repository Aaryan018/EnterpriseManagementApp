using Microsoft.EntityFrameworkCore;
namespace EnterpriseManagementApp;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


using EnterpriseManagementApp.Models;
//using Test2.Models.OldModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using EnterpriseManagementApp.Models.Rentals;

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
    public DbSet<AssetInvoice> AssetInvoices { get; set; }

    public DbSet<RentChange> RentChanges { get; set; }


    // Used to configure the model(s) before it is used to generate the database schema or to map entities to db tables; is called when EF Core
    // is building the model during app startup
    // modelBuilder param provides API's for configuring entities and relationships
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
            .ToTable("Employees");

        modelBuilder.Entity<Customer>()
            .ToTable("Customers");


        

        // Configuring the many-to-many relationship using the join table OccupancyHistory (Customer <->  Asset)
        modelBuilder.Entity<OccupancyHistory>()
            .HasKey(oh => new { oh.CustomerId, oh.AssetId });  // Composite key for the join table

        modelBuilder.Entity<OccupancyHistory>()
            .HasOne(oh => oh.Customer)
            .WithMany(r => r.OccupancyHistories)
            .HasForeignKey(oh => oh.CustomerId);

        modelBuilder.Entity<OccupancyHistory>()
            .HasOne(oh => oh.Asset)
            .WithMany(a => a.OccupancyHistories)
            .HasForeignKey(oh => oh.AssetId)
            .OnDelete(DeleteBehavior.NoAction);

        // Configuring the 1-to-many relationship between Asset and RentChange
        modelBuilder.Entity<RentChange>()
            .HasOne(rc => rc.Asset)  // RentChange has one Asset
            .WithMany(a => a.RentChanges)  // Asset can have many RentChanges
            .HasForeignKey(rc => rc.AssetId)  // Foreign key in RentChange
            .OnDelete(DeleteBehavior.Cascade);  // You can change the delete behavior as needed

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .ToTable("Employees");

            modelBuilder.Entity<Customer>()
                .ToTable("Customers");
        // Configure many-to-many relationship with a custom join table name
        modelBuilder.Entity<AppEvent>()
            .HasMany(e => e.Customers)
            .WithMany(c => c.Events);

        // Example of linking Event -> Service
        modelBuilder.Entity<AppEvent>()
            .HasOne(e => e.Service)
            .WithMany(s => s.Events)
            .HasForeignKey(e => e.ServiceId);

        // Attendance -> Event
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

        // Configure the relationship: One OccupancyHistory has many AssetInvoices
        modelBuilder.Entity<AssetInvoice>()
            .HasKey(ai => ai.AssetInvoiceId); // Primary key for AssetInvoice (separate from composite FK)

        // Define the composite foreign key relationship between AssetInvoice and OccupancyHistory
        modelBuilder.Entity<AssetInvoice>()
            .HasOne(ai => ai.OccupancyHistory)
            .WithMany(oh => oh.AssetInvoices)
            .HasForeignKey(ai => new { ai.CustomerId, ai.AssetId });  // Composite foreign key

        // Optionally, set delete behavior if required
        modelBuilder.Entity<AssetInvoice>()
            .HasOne(ai => ai.OccupancyHistory)
            .WithMany(oh => oh.AssetInvoices)
            .HasForeignKey(ai => new { ai.CustomerId, ai.AssetId })
            .OnDelete(DeleteBehavior.Cascade);


        // invokes base class implementation of the 'OnModelCreating' method; 
        base.OnModelCreating(modelBuilder);
    }


public DbSet<AppEvent> AppEvent { get; set; } = default!;
}

