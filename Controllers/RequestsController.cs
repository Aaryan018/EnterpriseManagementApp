using EnterpriseManagementApp;
using EnterpriseManagementApp.Enums;
using EnterpriseManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EnterpriseManagementApp.Controllers
{
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RequestsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Index()
        {
            var leaveRequests = await _context.LeaveRequests
                .Include(lr => lr.Employee)
                .ToListAsync();
            return View(leaveRequests);
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequests
                .Include(lr => lr.Employee)
                .FirstOrDefaultAsync(lr => lr.Id == id);

            if (leaveRequest == null)
            {
                return NotFound();
            }

            return View(leaveRequest);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Fetch Employee if it exists
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == userId);

            // Create LeaveRequest with fallback to ApplicationUser data
            var leaveRequest = new LeaveRequest
            {
                EmployeeId = userId,
                Employee = employee ?? new Employee // Use Employee if exists, else fallback
                {
                    Id = userId,
                    FullName = user.FullName ?? "Unknown", // Fallback from ApplicationUser
                    Email = user.Email ?? "no-email@example.com"
                },
                ApprovalStatus = false,
                Paid = true
            };

            // If no Employee exists, add an error but still show the form
            if (employee == null)
            {
                ModelState.AddModelError("", "You are not registered as an employee. Please contact an administrator to submit leave requests.");
            }

            return View(leaveRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([Bind("StartDate,EndDate,Type")] LeaveRequest leaveRequest)
        {
            // Get Current User ID
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "User ID not found. Please log in again.");
                return View(leaveRequest);
            }

            // Get User from Identity
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found in the system.");
                return View(leaveRequest);
            }

            // Set Employee ID for the Request
            leaveRequest.EmployeeId = userId;
            leaveRequest.ApprovalStatus = false;
            leaveRequest.Paid = true;

            // Ensure Employee Exists
            var employeeExists = await _context.Employees.AnyAsync(e => e.Id == userId);
            if (!employeeExists)
            {
                ModelState.AddModelError("", "You are not registered as an employee. Please contact an administrator.");
                return View(leaveRequest);
            }

            // Debugging ModelState Errors
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                System.Diagnostics.Debug.WriteLine("ModelState Errors: " + string.Join(", ", errors));
                return View(leaveRequest);
            }

            // Save Request
            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

            // Redirect to Index with Success Message
            TempData["SuccessMessage"] = "Leave request submitted successfully!";
            return RedirectToAction("Index", "Requests");
        }


        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Approve(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            leaveRequest.ApprovalStatus = true;
            _context.Update(leaveRequest);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Deny(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            leaveRequest.ApprovalStatus = false;
            _context.Update(leaveRequest);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}