using Microsoft.EntityFrameworkCore;
using EnterpriseManagementApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;

namespace EnterpriseManagementApp.Data
{
    public class ManageHousingDbContext : IdentityDbContext<ApplicationUser>
    {
        public ManageHousingDbContext(DbContextOptions<ManageHousingDbContext> options) : base(options) { }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<Renter> Renters { get; set; }
        public DbSet<RentChange> RentChanges { get; set; }
        public DbSet<OccupancyHistory> OccupancyHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define Primary Keys
            modelBuilder.Entity<Asset>().HasKey(a => a.AssetId);
            modelBuilder.Entity<Renter>().HasKey(r => r.RenterId);
            modelBuilder.Entity<RentChange>().HasKey(rc => rc.HistoryRentId);
            modelBuilder.Entity<OccupancyHistory>().HasKey(o => o.OccupancyId);

            // Configure Asset
            modelBuilder.Entity<Asset>(entity =>
            {
                // AssetId is a string, and we manage its generation (not database-generated)
                entity.Property(a => a.AssetId)
                      .ValueGeneratedNever(); // Application generates the GUID as a string

                // Decimal precision for Value
                entity.Property(a => a.Value)
                      .HasColumnType("decimal(18,2)");

                // Convert DateOnly to DateTime for database storage
                entity.Property(a => a.CreatedAt)
                      .HasColumnType("date")
                      .HasConversion(
                          v => v.ToDateTime(TimeOnly.MinValue), // DateOnly -> DateTime
                          v => DateOnly.FromDateTime(v)); // DateTime -> DateOnly

                entity.Property(a => a.UpdatedAt)
                      .HasColumnType("date")
                      .HasConversion(
                          v => v.ToDateTime(TimeOnly.MinValue),
                          v => DateOnly.FromDateTime(v));
            });

            // Configure Renter
            modelBuilder.Entity<Renter>(entity =>
            {
                entity.Property(r => r.RenterId)
                      .ValueGeneratedNever(); // Assuming RenterId is a Guid managed by the app
            });

            // Configure RentChange
            modelBuilder.Entity<RentChange>(entity =>
            {
                entity.Property(rc => rc.HistoryRentId)
                      .ValueGeneratedNever(); // Assuming HistoryRentId is a Guid managed by the app

                entity.Property(rc => rc.AssetId)
                      .HasConversion(
                          v => v, // AssetId is already a string
                          v => v); // No conversion needed
            });

            // Configure OccupancyHistory
            modelBuilder.Entity<OccupancyHistory>(entity =>
            {
                entity.Property(o => o.OccupancyId)
                      .ValueGeneratedNever(); // Assuming OccupancyId is a Guid managed by the app

                entity.Property(o => o.AssetId)
                      .HasConversion(
                          v => v,
                          v => v);

                // Ensure Start is a DateTime or Date in the database
                entity.Property(o => o.Start)
                      .HasColumnType("date")
                      .HasConversion(
                          v => v.ToDateTime(TimeOnly.MinValue),
                          v => DateOnly.FromDateTime(v));

                // Ensure End is nullable DateOnly
                entity.Property(o => o.End)
                      .HasColumnType("date")
                      .HasConversion(
                          v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                          v => v.HasValue ? DateOnly.FromDateTime(v.Value) : null);
            });

            // Define Relationships
            modelBuilder.Entity<Asset>()
                .HasOne<Renter>()
                .WithMany()
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OccupancyHistory>()
                .HasOne(o => o.Asset)
                .WithMany(a => a.Occupancies)
                .HasForeignKey(o => o.AssetId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RentChange>()
                .HasOne(rc => rc.Asset)
                .WithMany(a => a.RentChanges)
                .HasForeignKey(rc => rc.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure Unique Constraints
            modelBuilder.Entity<OccupancyHistory>()
                .HasIndex(o => new { o.AssetId, o.Start })
                .IsUnique();
        }
    }
}