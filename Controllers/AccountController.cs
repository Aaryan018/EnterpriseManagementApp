using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EnterpriseManagementApp.Models;
using System.Threading.Tasks;
//using EnterpriseManagementApp.Models.Authentication;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Diagnostics;

namespace EnterpriseManagementApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignIn model)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is invalid.");
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                Console.WriteLine($"User with email {model.Email} not found.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Console.WriteLine($"User {user.Email} logged in with roles: {string.Join(", ", roles)}");

                if (roles.Contains("Manager") || roles.Contains("Customer"))
                {
                    // Redirect both Manager and Client to Home/Index
                    Console.WriteLine("Redirecting to Home/Index for Manager or Client");
                    return RedirectToAction("Index", "Home");
                }

                Console.WriteLine("No specific role matched, redirecting to SignIn.");
                return RedirectToAction("SignIn", "Account");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            Console.WriteLine($"Login failed for {model.Email}. Result: {result}");
            return View(model);
        }


        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUp model)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is invalid.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("Validation Error: " + error.ErrorMessage);
                }

                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { field = x.Key, errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                    .ToList();

                return Json(new { success = false, message = "Validation failed 1.", errors });
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                EmergencyContact = model.EmergencyContact
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(model.Role))
                {
                    Console.WriteLine($"Role '{model.Role}' does not exist. Creating it now.");
                    await _roleManager.CreateAsync(new IdentityRole(model.Role));
                }

                await _userManager.AddToRoleAsync(user, model.Role);
                await _signInManager.SignInAsync(user, isPersistent: false);

                Console.WriteLine($"User {user.Email} signed up as {model.Role}");

                if (model.Role == "Manager")
                {
                    Console.WriteLine("Redirecting to Home/Index for Manager");
                    return RedirectToAction("Index", "Home"); // Redirect Manager to Home/Index (Common Dashboard)
                }
                else if (model.Role == "Customer")
                {
                    Console.WriteLine("Redirecting to Renters/Index for Client");
                    return RedirectToAction("Index", "Renters"); // Updated for Client
                }

                Console.WriteLine("No specific role matched, redirecting to SignIn.");
                return RedirectToAction("SignIn", "Account");
            } else
            {
                // User creation failed, prepare error messages
                var errorMessages = result.Errors.Select(error => error.Description).ToList();

                // Return the errors as a JSON response
                return Json(new { success = false, errors = errorMessages });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Sign out the user from Identity
            await _signInManager.SignOutAsync();

            // Explicitly sign out any authentication cookies
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            Console.WriteLine("User logged out successfully, redirecting to SignIn.");

            // Redirect to the SignIn page after logout
            return RedirectToAction("SignIn", "Account");
        }

        public IActionResult AccessDenied()
        {
            Console.WriteLine("Access denied page accessed.");
            return View();
        }
    }
}
