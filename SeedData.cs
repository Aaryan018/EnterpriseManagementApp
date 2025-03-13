using EnterpriseManagementApp.Models;
using EnterpriseManagementApp.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace EnterpriseManagementApp
{
    public static class SeedData
    {
        public static async Task InitializeAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Create default roles
            string[] roleNames = { "Admin", "User", "Manager" };  // Adding Manager role as well
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create admin user
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, "Admin@1234");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // You can also add a default manager if needed
            var managerEmail = "manager@example.com";
            var managerUser = await userManager.FindByEmailAsync(managerEmail);
            if (managerUser == null)
            {
                managerUser = new ApplicationUser
                {
                    UserName = managerEmail,
                    Email = managerEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(managerUser, "Manager@1234");
                await userManager.AddToRoleAsync(managerUser, "Manager");
            }
        }
    }
}
