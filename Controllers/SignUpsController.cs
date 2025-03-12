//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Identity;
//using EnterpriseManagementApp.Models.Authentication;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;
//using System.Security.Claims;
//using EnterpriseManagementApp.Models;

//namespace EnterpriseManagementApp.Controllers
//{
//    public class SignUpsController : Controller
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly RoleManager<IdentityRole> _roleManager;
//        private readonly ILogger<SignUpsController> _logger;

//        public SignUpsController(
//            UserManager<ApplicationUser> userManager,
//            SignInManager<ApplicationUser> signInManager,
//            RoleManager<IdentityRole> roleManager,
//            ILogger<SignUpsController> logger)
//        {
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _roleManager = roleManager;
//            _logger = logger;
//        }

//        // GET: SignUps/Index
//        [HttpGet]
//        public IActionResult Index()
//        {
//            return View();
//        }

//        // POST: SignUps/Index
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Index(SignUp model)
//        {
//            if (!ModelState.IsValid)
//            {
//                _logger.LogWarning("Model state invalid during signup for {Email}.", model.Email);
//                return View(model);
//            }

//            var user = new ApplicationUser
//            {
//                UserName = model.Email,
//                Email = model.Email,
//                Role = model.Role,
//                Module = model.Module
//            };

//            // Create the user and log the outcome
//            var result = await _userManager.CreateAsync(user, model.Password);
//            if (result.Succeeded)
//            {
//                _logger.LogInformation("User {Email} created successfully with ID: {UserId}.", model.Email, user.Id);

//                // Verify user exists in the database
//                var createdUser = await _userManager.FindByEmailAsync(model.Email);
//                if (createdUser == null || createdUser.Id != user.Id)
//                {
//                    _logger.LogError("User {Email} not found in database after creation.", model.Email);
//                    ModelState.AddModelError(string.Empty, "User creation failed unexpectedly.");
//                    return View(model);
//                }

//                // Ensure role exists
//                if (!await _roleManager.RoleExistsAsync(model.Role))
//                {
//                    var role = new IdentityRole(model.Role);
//                    var roleResult = await _roleManager.CreateAsync(role);
//                    if (!roleResult.Succeeded)
//                    {
//                        _logger.LogError("Failed to create role {Role} for user {Email}.", model.Role, model.Email);
//                        foreach (var error in roleResult.Errors)
//                        {
//                            ModelState.AddModelError(string.Empty, error.Description);
//                        }
//                        await _userManager.DeleteAsync(user); // Clean up
//                        return View(model);
//                    }
//                    _logger.LogInformation("Role {Role} created successfully.", model.Role);
//                }

//                // Assign the role
//                var addRoleResult = await _userManager.AddToRoleAsync(user, model.Role);
//                if (!addRoleResult.Succeeded)
//                {
//                    _logger.LogError("Failed to assign role {Role} to user {Email}. Errors: {Errors}",
//                        model.Role, model.Email, string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
//                    foreach (var error in addRoleResult.Errors)
//                    {
//                        ModelState.AddModelError(string.Empty, error.Description);
//                    }
//                    await _userManager.DeleteAsync(user); // Clean up
//                    return View(model);
//                }

//                // Add Module as a claim
//                var claimResult = await _userManager.AddClaimAsync(user, new Claim("Module", model.Module));
//                if (!claimResult.Succeeded)
//                {
//                    _logger.LogWarning("Failed to add claim 'Module'={Module} for user {Email}.", model.Module, model.Email);
//                    foreach (var error in claimResult.Errors)
//                    {
//                        ModelState.AddModelError(string.Empty, error.Description);
//                    }
//                }

//                _logger.LogInformation("User {Email} registered successfully with Role: {Role} and Module: {Module}.",
//                    model.Email, model.Role, model.Module);

//                await _signInManager.SignInAsync(user, isPersistent: false);
//                return RedirectToAction("Index", "Home");
//            }

//            // Log user creation failure
//            _logger.LogError("User creation failed for {Email}. Errors: {Errors}",
//                model.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
//            foreach (var error in result.Errors)
//            {
//                ModelState.AddModelError(string.Empty, error.Description);
//            }

//            return View(model);
//        }
//    }
//}