using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EnterpriseManagementApp.Models;
using System.Threading.Tasks;
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

        // GET: /Account/SignIn
        public IActionResult SignIn()
        {
            return View();
        }

        // POST: /Account/SignIn
        [HttpPost]
        public async Task<IActionResult> SignIn(SignIn model)
        {
            Console.WriteLine("SignIn POST action hit.");

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

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                Console.WriteLine($"User {user.Email} logged in with roles: {string.Join(", ", userRoles)}");

                if (userRoles.Contains("Manager"))
                {
                    Console.WriteLine("Redirecting to Home/Index for Manager");
                    return RedirectToAction("Index", "Home");
                }
                else if (userRoles.Contains("Customer")) // Updated to "Customer"
                {
                    Console.WriteLine("Redirecting to Customers/Index for Customer");
                    return RedirectToAction("Index", "Customers");
                }

                Console.WriteLine("No specific role matched, redirecting to Home/Index.");
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            Console.WriteLine($"Login failed for {model.Email}. Result: {result}");
            return View(model);
        }

        // GET: /Account/SignUp
        public IActionResult SignUp()
        {
            return View();
        }

        // POST: /Account/SignUp
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

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { field = x.Key, errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                        .ToList();
                    return Json(new { success = false, message = "Validation failed.", errors });
                }
                return View(model);
            }

            var user = new ApplicationUser();

            if (model.Role == "Customer")
            {
                user = new Customer
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    EmergencyContact = model.EmergencyContact,
                    FamilyDoctor = model.FamilyDoctor,
                    Role = model.Role
                };
            } else
            {
                user = new Employee
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    EmergencyContact = model.EmergencyContact,
                    Role = model.Role
                };
            }



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
                    return RedirectToAction("Index", "Home");
                }
                else if (model.Role == "Customer") // Updated to "Customer"
                {
                    Console.WriteLine("Redirecting to Customers/Index for Customer");
                    return RedirectToAction("Index", "Customers");
                }

                Console.WriteLine("No specific role matched, redirecting to SignIn.");
                return RedirectToAction("SignIn", "Account");
            }

            // Handle user creation failure
            var errorMessages = result.Errors.Select(error => error.Description).ToList();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, errors = errorMessages });
            }
            foreach (var error in errorMessages)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            Console.WriteLine("User logged out successfully, redirecting to SignIn.");
            return RedirectToAction("SignIn", "Account");
        }

        // GET: /Account/AccessDenied
        public IActionResult AccessDenied()
        {
            Console.WriteLine("Access denied page accessed.");
            return View();
        }
    }
}