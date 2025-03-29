using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnterpriseManagementApp.Models;
using System.Security.Claims;

namespace EnterpriseManagementApp.Controllers
{
    [Authorize] // Require authentication for all actions
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Customers (Filtered based on role)
        [Authorize(Roles = "Manager,Customer")] // Updated to "Customer"
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Manager"))
            {
                // Managers can see all users with the "Customer" role
                var customers = await _userManager.GetUsersInRoleAsync("Customer");
                Console.WriteLine($"Manager view: {customers.Count} customers found.");
                return View(customers);
            }
            else if (User.IsInRole("Customer"))
            {
                // Customers can only see their own data
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found for Customer.");
                    return Unauthorized("User ID not found.");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    Console.WriteLine($"User not found for ID: {userId}");
                    return NotFound("User not found.");
                }

                var model = new List<ApplicationUser> { user };
                Console.WriteLine($"Customer model: {string.Join(", ", model.Select(u => u.Email))}");
                return View(model);
            }

            return Unauthorized("User role not recognized.");
        }

        // GET: Customers/Details/5 (Viewable by both Managers and Customers)
        [Authorize(Roles = "Manager,Customer")] // Updated to "Customer"
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // For customers, ensure they can only view their own record
            if (User.IsInRole("Customer"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != id)
                {
                    return Unauthorized("You can only view your own data.");
                }
            }

            return View(user);
        }

        // GET: Customers/Create (Manager-only)
        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create([Bind("Id,FullName,Address,PhoneNumber,Email,EmergencyContact")] ApplicationUser user, string password)
        {
            if (ModelState.IsValid)
            {
                user.UserName = user.Email; // Set username to email
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer"); // Updated to "Customer"
                    return RedirectToAction(nameof(Index));
                }
                AddErrors(result);
            }
            return View(user);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // GET: Customers/Edit/5 (Manager-only)
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FullName,Address,PhoneNumber,Email,EmergencyContact")] ApplicationUser user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByIdAsync(id);
                if (existingUser != null)
                {
                    existingUser.FullName = user.FullName;
                    existingUser.Address = user.Address;
                    existingUser.PhoneNumber = user.PhoneNumber;
                    existingUser.Email = user.Email;
                    existingUser.EmergencyContact = user.EmergencyContact;

                    var result = await _userManager.UpdateAsync(existingUser);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    AddErrors(result);
                }
            }
            return View(user);
        }

        // GET: Customers/Delete/5 (Manager-only)
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}