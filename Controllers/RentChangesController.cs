using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EnterpriseManagementApp.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseManagementApp.Controllers
{
    [Authorize(Roles = "Customer")]
    public class RentChangesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RentChangesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: RentChanges/Index (Show only the current customer's requests)
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found.");
            }

            var rentChanges = await _context.RentChanges
                .Where(rc => rc.UserId == userId)
                .Include(rc => rc.Asset)
                .ToListAsync();

            return View(rentChanges);
        }

        // GET: RentChanges/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found.");
            }

            var rentChange = await _context.RentChanges
                .Include(rc => rc.Asset)
                .FirstOrDefaultAsync(rc => rc.RentChangeId == id && rc.UserId == userId);

            if (rentChange == null)
            {
                return NotFound("Rent change request not found or you do not have access to it.");
            }

            return View(rentChange);
        }

        public IActionResult Create()
        {
            var assets = _context.Assets.Where(a => a.Status).ToList();
            if (!assets.Any())
            {
                ModelState.AddModelError("", "No approved assets available to select. Please contact an administrator.");
            }

            ViewData["AssetId"] = new SelectList(assets, "AssetId", "Address");
            return View(new RentChange());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RentChangeId,AssetId,ChangeDate,OldRate,NewRate,Reason,UserId")] RentChange rentChange)
        {
            ModelState.Remove("User");
            ModelState.Remove("Asset");

            // Validate form
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Validation Error for {state.Key}: {error.ErrorMessage}");
                    }
                }
                ViewData["AssetId"] = new SelectList(_context.Assets.ToList(), "AssetId", "Address", rentChange.AssetId);
                return View(rentChange);
            }

            // Ensure UserId is correctly assigned and exists
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Unable to identify the current user.");
                ViewData["AssetId"] = new SelectList(_context.Assets.ToList(), "AssetId", "Address", rentChange.AssetId);
                return View(rentChange);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found in the system.");
                ViewData["AssetId"] = new SelectList(_context.Assets.ToList(), "AssetId", "Address", rentChange.AssetId);
                return View(rentChange);
            }
            rentChange.UserId = userId;

            // Validate asset exists
            var assetExists = await _context.Assets.AnyAsync(a => a.AssetId == rentChange.AssetId);
            if (!assetExists || rentChange.AssetId == Guid.Empty)
            {
                ModelState.AddModelError("AssetId", "The selected asset is invalid or no longer exists.");
                ViewData["AssetId"] = new SelectList(_context.Assets.ToList(), "AssetId", "Address", rentChange.AssetId);
                return View(rentChange);
            }

            // Set additional properties and save
            rentChange.RentChangeId = Guid.NewGuid();
            rentChange.Status = "Pending";
            rentChange.SubmittedDate = DateTime.Now;

            _context.Add(rentChange);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Rent change saved with ID: {rentChange.RentChangeId}");

            TempData["SuccessMessage"] = "Rent change request submitted successfully!";
            return RedirectToAction("Index", "Customers");
        }
    }
}