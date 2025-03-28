using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EnterpriseManagementApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EnterpriseManagementApp.Controllers
{
    [Authorize(Roles = "Manager, Employee")] // Restrict to managers only
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var rentChanges = await _context.RentChanges
                .Include(r => r.Asset)
                .Include(r => r.User)
                .ToListAsync();
            return View(rentChanges);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveRequest(Guid id)
        {
            var rentChange = await _context.RentChanges.FindAsync(id);
            if (rentChange == null)
            {
                _logger.LogWarning("Rent change request not found for ID: {Id}", id);
                return NotFound();
            }

            rentChange.Status = "Approved";
            rentChange.ProcessedDate = System.DateTime.Now;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Rent change request {Id} approved by manager.", id);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RejectRequest(Guid id)
        {
            var rentChange = await _context.RentChanges.FindAsync(id);
            if (rentChange == null)
            {
                _logger.LogWarning("Rent change request not found for ID: {Id}", id);
                return NotFound();
            }

            rentChange.Status = "Rejected";
            rentChange.ProcessedDate = System.DateTime.Now;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Rent change request {Id} rejected by manager.", id);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}