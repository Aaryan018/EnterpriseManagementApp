using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using EnterpriseManagementApp.Data;
using EnterpriseManagementApp.Models;
using EnterpriseManagementApp.Models.Authentication;
using System.Collections.Generic;

namespace EnterpriseManagementApp.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Employees/Index (List all employees for Managers and Admins)
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Include(e => e.Manager)
                .ToListAsync();
            return View(employees);
        }

        // GET: Employees/Manage (Updated for "Manage Employee" button)
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Manage()
        {
            var currentUser = await GetCurrentEmployeeAsync();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            IQueryable<Employee> query = _context.Employees.Include(e => e.Manager);

            // Admins and Managers see all employees
            if (currentUser.IsAdmin || currentUser.IsManager)
            {
                var employees = await query.ToListAsync();
                return View(employees);
            }
            // Employees see only their own record
            else
            {
                var employee = await query.FirstOrDefaultAsync(e => e.EmployeeID == currentUser.EmployeeID);
                if (employee == null)
                {
                    return NotFound("No employee data found for your account.");
                }
                return View(new List<Employee> { employee });
            }
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentEmployeeAsync();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var employee = await _context.Employees
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(m => m.EmployeeID == id);

            if (employee == null)
            {
                return NotFound();
            }

            // Admins and Managers can see details of any employee
            if (currentUser.IsAdmin || currentUser.IsManager)
            {
                return View(employee);
            }

            // Employees can only see their own profile
            if (currentUser.CanView(employee))
            {
                return View(employee);
            }

            return Unauthorized();
        }

        // GET: Employees/Create (Only Admins can create)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["ReportsTo"] = new SelectList(_context.Employees, "EmployeeID", "Name");
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("EmployeeID,Name,Address,EmergencyContact,JobTitle,EmploymentType,Salary,HourlyRate,ReportsTo,Email,PhoneNumber,HireDate,Status,Role,UserId")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    employee.UserId = currentUser.Id; // Link to the current user's ApplicationUser
                }
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Manage));
            }
            ViewData["ReportsTo"] = new SelectList(_context.Employees, "EmployeeID", "Name", employee.ReportsTo);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentEmployeeAsync();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            // Admins can edit any employee, Employees can edit only their own
            if (!currentUser.IsAdmin && !currentUser.CanView(employee))
            {
                return Unauthorized();
            }

            ViewData["ReportsTo"] = new SelectList(_context.Employees, "EmployeeID", "Name", employee.ReportsTo);
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeID,Name,Address,EmergencyContact,JobTitle,EmploymentType,Salary,HourlyRate,ReportsTo,Email,PhoneNumber,HireDate,Status,Role,UserId")] Employee employee)
        {
            if (id != employee.EmployeeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Manage));
            }
            ViewData["ReportsTo"] = new SelectList(_context.Employees, "EmployeeID", "Name", employee.ReportsTo);
            return View(employee);
        }

        // GET: Employees/Delete/5 (Only Admins can delete)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(m => m.EmployeeID == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Manage));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeID == id);
        }

        private async Task<Employee> GetCurrentEmployeeAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return null;
            }

            return await _context.Employees.FirstOrDefaultAsync(e => e.UserId == user.Id);
        }
    }
}