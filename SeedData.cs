﻿using EnterpriseManagementApp.Models;
using EnterpriseManagementApp.Models.Rentals;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
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
            string[] roles = { "Employee", "Manager", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine($"Role '{role}' created successfully.");
                }
            }

            // Seed Employee User
            if (userManager.Users.All(u => u.UserName != "Employee@enterprise.com"))
            {
                var employee = new Employee
                {
                    UserName = "Employee@enterprise.com",
                    Email = "Employee@enterprise.com",
                    EmailConfirmed = true,
                    FullName = "Emp Loyalee",
                    Address = "E way 1220",
                    PhoneNumber = "111-222-2828",
                    EmergencyContact = "911-911-9111",
                    Role = "Employee"
                };
                var result = await userManager.CreateAsync(employee, "Employee@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(employee, "Employee");
                    Console.WriteLine("Employee user created successfully.");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error creating Employee user: {error.Description}");
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
                    EmergencyContact = "911-911-9111",
                    Role = "Manager"
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
                    FamilyDoctor = "fred Boe",
                    Role = "Customer"
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
                    FamilyDoctor = "fred Boe",
                    Role = "Customer"
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
                // Specify the email address you want to find
                string targetEmail = "client@enterprise.com";

                // Find the customer with the given email
                var renter = dbContext.Customers.FirstOrDefault(c => c.Email == targetEmail);
                var assetTemp = dbContext.Assets.FirstOrDefault();

                if (renter != null && assetTemp != null)
                {
                    var OH = new OccupancyHistory
                    {
                        OccupancyHistoryId = Guid.NewGuid(),
                        CustomerId = renter.Id,
                        AssetId = assetTemp.AssetId,
                        Customer = renter,
                        Asset = assetTemp,
                        Start = DateOnly.FromDateTime(DateTime.Now),
                        End = DateOnly.FromDateTime(DateTime.Now.AddMonths(2)),
                        Paid = 0.00,
                        TotalDue = 0.00,
                        Status = "Approved"
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
                    Console.WriteLine("No renter and/or asset found to seed a OccupancyHistory. Please seed renter and/or asset first.");
                }
            }
            else
            {
                var assetCount = dbContext.Assets.Count();
                Console.WriteLine($"Found {assetCount} assets in the database.");
            }

            // Seed a Sample AssetInvoice
            if (!dbContext.AssetInvoices.Any())
            {
                var occupancyHistory = dbContext.OccupancyHistories.FirstOrDefault();

                if (occupancyHistory != null)
                {

                    var AI = new AssetInvoice
                    {
                        AssetInvoiceId = Guid.NewGuid(),
                        CustomerId = occupancyHistory.CustomerId,
                        AssetId = occupancyHistory.AssetId,
                        OccupancyHistory = occupancyHistory,
                        DatePaid = DateTime.Now,
                        AmmountPaid = 10
                    };

                    dbContext.AssetInvoices.Add(AI);
                    try
                    {
                        await dbContext.SaveChangesAsync();
                        Console.WriteLine($"Sample Asset Invoice created successfully");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error seeding Asset Invoice: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("No OccuapncyHistory. Please seed OccupancyHistory first.");
                }
            }
            else
            {
                var assetCount = dbContext.Assets.Count();
                Console.WriteLine($"Found {assetCount} assets in the database.");
            }

            // Seed RentChange Data - Removed RentAmount and RentChangeDate
            if (!dbContext.RentChanges.Any())
            {
                var customer = dbContext.Customers.FirstOrDefault();
                var asset = dbContext.Assets.FirstOrDefault();

                if (customer != null && asset != null)
                {
                    var rentChange = new RentChange
                    {
                        UserId = customer.Id, // Set the correct UserId here
                        // Add any other properties needed here
                    };

                    dbContext.RentChanges.Add(rentChange);
                    try
                    {
                        await dbContext.SaveChangesAsync();
                        Console.WriteLine($"RentChange created successfully for User={customer.FullName}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error seeding RentChange: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("No customer or asset found to seed RentChange. Please seed customer and asset first.");
                }
            }
            else
            {
                var rentChangeCount = dbContext.RentChanges.Count();
                Console.WriteLine($"Found {rentChangeCount} RentChange records in the database.");
            }
        }
    }


}

