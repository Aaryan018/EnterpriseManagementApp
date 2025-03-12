using EnterpriseManagementApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseManagementApp

{
    public class SeedData
    {

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

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
                var admin = new Employee
                {
                    UserName = "admin@enterprise.com",
                    Email = "admin@enterprise.com",
                    EmailConfirmed = true,
                    FullName = "Adam Min",
                    Address = "Adm Way 1220",
                    PhoneNumber = "111-222-2828",
                    EmergencyContact = "911-911-9111"
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
                var manager = new Employee
                {
                    UserName = "manager@enterprise.com",
                    Email = "manager@enterprise.com",
                    EmailConfirmed = true,
                    FullName = "Mana Ger",
                    Address = "Mng Way 4440",
                    PhoneNumber = "111-222-2828",
                    EmergencyContact = "911-911-9111"
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

            // Seed a Sample Asset
            if (!dbContext.Assets.Any())
            {

                var asset = new Asset
                {
                    AssetId = Guid.NewGuid(),
                    Name = "Sample Asset",
                    Type = "Residential",
                    RentRate = 50.00,
                    Description = "A sample asset for testing",
                    Address = "123 Test Street",
                    Status = true,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                    UpdatedAt = DateOnly.FromDateTime(DateTime.Now)
                };


                var asset2 = new Asset
                {
                    AssetId = Guid.NewGuid(),
                    Name = "Sample Asset 2 - OH Test",
                    Type = "Residential",
                    RentRate = 120.00,
                    Description = "A sample asset for testing",
                    Address = "123 Test Street",
                    Status = true,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                    UpdatedAt = DateOnly.FromDateTime(DateTime.Now)
                };

                dbContext.Assets.AddRange(asset, asset2);
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
                var assetCount = dbContext.Assets.Count();
                Console.WriteLine($"Found {assetCount} assets in the database.");
            }


            // Seed Customer User
            if (userManager.Users.All(u => u.UserName != "client@enterprise.com"))
            {
                var client = new Customer
                {
                    UserName = "client@enterprise.com",
                    Email = "client@enterprise.com",
                    EmailConfirmed = true,
                    FullName = "Clide Ent",
                    Address = "Clients Way @ 4440",
                    PhoneNumber = "111-222-2828",
                    EmergencyContact = "911-911-9111",
                    FamilyDoctor = "fred Boe"
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

            // Seed Customer User - for OH creation testing
            if (userManager.Users.All(u => u.UserName != "client2@enterprise.com"))
            {
                var client = new Customer
                {
                    UserName = "client2@enterprise.com",
                    Email = "client2@enterprise.com",
                    EmailConfirmed = true,
                    FullName = "Clide Ent - OH test",
                    Address = "Clients Way @ 5550",
                    PhoneNumber = "111-222-2828",
                    EmergencyContact = "911-911-9111",
                    FamilyDoctor = "fred Boe"
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

            // Seed a Sample OccupancyHistory
            if (!dbContext.OccupancyHistories.Any())
            {
                var renter = dbContext.Customers.FirstOrDefault();
                var assetTemp = dbContext.Assets.FirstOrDefault();

                if (renter != null && assetTemp != null)
                {
                    var OH = new OccupancyHistory
                    {
                        OccupancyHistoryId = Guid.NewGuid(),
                        CustomerId = renter.Id,
                        AssetId = assetTemp.AssetId,
                        Start = DateOnly.FromDateTime(DateTime.Now),
                        End = DateOnly.FromDateTime(DateTime.Now),
                        Paid = 0.00,
                        AmmountDue = 12.99,
                        Status = "Pending"
                    };

                    dbContext.OccupancyHistories.Add(OH);
                    try
                    {
                        await dbContext.SaveChangesAsync();
                        Console.WriteLine($"Sample OccupancyHistory created successfully: AssetName={assetTemp.Name}, CustomerName={renter.FullName}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error seeding OccupancyHistory: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("No renter and/or asset found to seed a OccuapncyHistory. Please seed renter and/or asset first.");
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
