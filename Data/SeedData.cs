using EnterpriseManagementApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseManagementApp.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var dbContext = serviceProvider.GetRequiredService<ManageHousingDbContext>();

            // Ensure roles are created
            string[] roles = { "Admin", "Manager", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine($"Role '{role}' created successfully.");
                }
            }

            // Seed Admin User
            if (userManager.Users.All(u => u.UserName != "admin@enterprise.com"))
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@enterprise.com",
                    Email = "admin@enterprise.com",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    Console.WriteLine("Admin user created successfully.");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error creating Admin user: {error.Description}");
                    }
                }
            }

            // Seed Manager User
            if (userManager.Users.All(u => u.UserName != "manager@enterprise.com"))
            {
                var manager = new ApplicationUser
                {
                    UserName = "manager@enterprise.com",
                    Email = "manager@enterprise.com",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(manager, "Manager@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(manager, "Manager");
                    Console.WriteLine("Manager user created successfully.");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error creating Manager user: {error.Description}");
                    }
                }
            }

            // Seed Customer User
            if (userManager.Users.All(u => u.UserName != "client@enterprise.com"))
            {
                var client = new ApplicationUser
                {
                    UserName = "client@enterprise.com",
                    Email = "client@enterprise.com",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(client, "Client@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(client, "Customer");
                    Console.WriteLine("Customer user created successfully.");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error creating Customer user: {error.Description}");
                    }
                }
            }

            // Seed a Renter
            if (!dbContext.Renters.Any())
            {
                var renter = new Renter
                {
                    RenterId = Guid.NewGuid(),
                    Name = "Test Renter",
                    Email = "renter@example.com",
                    ContactNumber = "123-456-7890",
                    Address = "123 Renter Street"
                };
                dbContext.Renters.Add(renter);
                try
                {
                    await dbContext.SaveChangesAsync();
                    Console.WriteLine($"Test Renter created successfully: RenterId={renter.RenterId}, Name={renter.Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding Renter: {ex.Message}");
                }
            }
            else
            {
                var renterCount = dbContext.Renters.Count();
                Console.WriteLine($"Found {renterCount} renters in the database.");
            }

            // Seed a Sample Asset
            if (!dbContext.Assets.Any())
            {
                var renter = dbContext.Renters.FirstOrDefault();
                if (renter != null)
                {
                    var asset = new Asset
                    {
                        AssetId = Guid.NewGuid().ToString(),
                        Name = "Sample Asset",
                        Type = "Residential",
                        Value = 1000.50m,
                        RentRate = 50.00,
                        Description = "A sample asset for testing",
                        Address = "123 Test Street",
                        Status = true,
                        CustomerId = renter.RenterId,
                        CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                        UpdatedAt = DateOnly.FromDateTime(DateTime.Now)
                    };
                    dbContext.Assets.Add(asset);
                    try
                    {
                        await dbContext.SaveChangesAsync();
                        Console.WriteLine($"Sample Asset created successfully: AssetId={asset.AssetId}, Name={asset.Name}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error seeding Asset: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("No renters found to seed an asset. Please seed a renter first.");
                }
            }
            else
            {
                var assetCount = dbContext.Assets.Count();
                Console.WriteLine($"Found {assetCount} assets in the database.");
            }
        }
    }
}
