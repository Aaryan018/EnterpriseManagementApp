using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using EnterpriseManagementApp.Models;
using EnterpriseManagementApp.Models.Authentication;

namespace EnterpriseManagementApp.Controllers
{
    public class SignInsController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SignInsController> _logger;

        public SignInsController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<SignInsController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                _logger.LogInformation("User already authenticated, redirecting to Home");
                return RedirectToAction("Index", "Home");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View(new SignIn());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignIn model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state invalid for login attempt");
                return View("Index", model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("Login attempt for non-existent email: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View("Index", model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName, // Use UserName instead of user object
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} logged in successfully", model.Email);

                // Null-safe logging
                _logger.LogInformation("User details - Role: {Role}, Module: {Module}, CreatedBy: {CreatedBy}",
                    user.Role ?? "Not Assigned",
                    user.Module ?? "Not Assigned",
                    user.CreatedBy ?? "System");

                return RedirectToLocal(returnUrl) ?? await RedirectToRoleBasedPage(user);
            }

            if (result.RequiresTwoFactor)
            {
                _logger.LogInformation("2FA required for {Email}", model.Email);
                return RedirectToAction("LoginWith2fa", new { ReturnUrl = returnUrl });
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Account {Email} locked out", model.Email);
                ModelState.AddModelError(string.Empty, "Account locked. Contact support.");
                return View("Index", model);
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your credentials.");
            return View("Index", model);
        }

        private IActionResult? RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return null;
        }

        private async Task<IActionResult> RedirectToRoleBasedPage(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            _logger.LogInformation("User {UserName} has roles: {Roles}", user.UserName, roles);

            if (roles.Contains("Admin"))
                return RedirectToAction("AdminDashboard", "Admin");
            if (roles.Contains("Manager"))
                return RedirectToAction("ManagerDashboard", "Manager");
            if (roles.Contains("Employee"))
                return RedirectToAction("EmployeeDashboard", "Employee");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult LoginWith2fa(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out");
            return RedirectToAction("Index", "Home");
        }
    }
}