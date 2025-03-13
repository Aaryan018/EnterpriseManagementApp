using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EnterpriseManagementApp.Models;
using EnterpriseManagementApp.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace EnterpriseManagementApp.Controllers
{
    [Authorize] // Ensure that all actions require authentication
    public class ProfilesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfilesController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Profile action to show user details
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch roles for the current user
            var roles = await _userManager.GetRolesAsync(currentUser);
            var role = roles.FirstOrDefault() ?? "No role assigned"; // Default to "No role assigned" if none exists
            var module = currentUser.Module ?? "No module assigned"; // Default to "No module assigned" if none exists

            // Create a profile model instance
            var profile = new Profile
            {
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName,
                Email = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber,
                Address = currentUser.Address,
                Role = role, // Include Role
                Module = module // Include Module
            };

            // Return the Index view with the profile model
            return View("Index", profile);  // Explicitly specify 'Index' view
        }

        // Logout action to log the user out
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); // Sign out the user
            return RedirectToAction("Index", "Home"); // Redirect to home page or login page
        }
    }
}