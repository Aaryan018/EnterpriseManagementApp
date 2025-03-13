using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using EnterpriseManagementApp.Data;
using EnterpriseManagementApp.Models;
using EnterpriseManagementApp.Models.Authentication;
//using EnterpriseManagementApp.Models.Employees;
using EnterpriseManagementApp.Models.Rentals;

namespace EnterpriseManagementApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;

        public HomeController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<HomeController> logger,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogWarning("Authenticated user not found for User.Identity.Name: {UserName}", User?.Identity?.Name);
                    return RedirectToAction("Index", "SignIns");
                }

                _logger.LogInformation("Loading dashboard for user {Email} with Role: {Role}, Module: {Module}",
                    currentUser.Email, currentUser.Role, currentUser.Module);

                var model = new DashboardViewModel
                {
                    UserFullName = $"{currentUser.FirstName ?? "Unknown"} {currentUser.LastName ?? "Unknown"}",
                    UserEmail = currentUser.Email ?? "No Email",
                    CanViewEmployeeList = currentUser.Role == "Manager" // Flag for Manager access to employee list
                };

                if (currentUser.Role == "Manager")
                {
                    await LoadManagerDashboard(model);
                }
                else if (currentUser.Module == "Housing")
                {
                    await LoadHousingDashboard(model);
                }
                else if (currentUser.Module == "Care")
                {
                    // Placeholder for future Care module data
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data for user {UserName}", User?.Identity?.Name);
                if (_environment.IsDevelopment())
                {
                    throw; // Show detailed error in Development mode
                }
                TempData["ErrorMessage"] = "Error loading dashboard data. Please try again later.";
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed.");
            }
            return RedirectToAction("Index", "SignIns");
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        // Helper Methods for Dashboard Data
        private async Task LoadManagerDashboard(DashboardViewModel model)
        {
            //model.EmployeeCount = await SafeCountAsync(_context.Employees);
            //model.ActiveShifts = await SafeCountAsync(_context.Shifts.Where(s => s.EndTime > DateTime.Now));
            //model.PendingCertifications = await SafeCountAsync(_context.EmployeeCertifications
            //    .Where(ec => ec.ExpiryDate < DateTime.Now.AddDays(30)));
            //model.TotalPayroll = await SafeSumAsync(_context.Payrolls.Select(p => (decimal?)p.TotalAmount ?? 0m));
            //model.UpcomingPayments = await SafeCountAsync(_context.PayrollDetails
            //    .Where(pd => pd.PaymentDate > DateTime.Now));
            model.ActiveRentals = await SafeCountAsync(_context.RentHistories
                .Where(rh => rh.RentDate <= DateTime.Now));
            model.LeaseExpiring = await SafeCountAsync(_context.LeaseAgreements
                .Where(la => la.EndDate < DateTime.Now.AddDays(7)));
        }

        private async Task LoadHousingDashboard(DashboardViewModel model)
        {
            model.ActiveRentals = await SafeCountAsync(_context.RentHistories
                .Where(rh => rh.RentDate <= DateTime.Now));
            model.LeaseExpiring = await SafeCountAsync(_context.LeaseAgreements
                .Where(la => la.EndDate < DateTime.Now.AddDays(7)));
        }

        // Helper method to safely handle database counts
        private async Task<int> SafeCountAsync(IQueryable<object> queryable)
        {
            try
            {
                return await queryable.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to count records, returning 0");
                return 0;
            }
        }

        // Helper method to safely handle database sums
        private async Task<decimal> SafeSumAsync(IQueryable<decimal> queryable)
        {
            try
            {
                return await queryable.SumAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to sum records, returning 0m");
                return 0m;
            }
        }
    }
}