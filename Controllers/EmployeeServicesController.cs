using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnterpriseManagementApp.Data;
using EnterpriseManagementApp.Models.Employees;

namespace EnterpriseManagementApp.Controllers
{
    public class EmployeeServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EmployeeServices
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.EmployeeServices
                .Include(e => e.Employee)
                .Include(e => e.Service);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: EmployeeServices/Details/5
        public async Task<IActionResult> Details(int? employeeId, int? serviceId)
        {
            if (employeeId == null || serviceId == null)
            {
                return NotFound();
            }

            var employeeService = await _context.EmployeeServices
                .Include(e => e.Employee)
                .Include(e => e.Service)
                .FirstOrDefaultAsync(m => m.EmployeeID == employeeId && m.ServiceID == serviceId);

            return employeeService == null ? NotFound() : View(employeeService);
        }

        // GET: EmployeeServices/Create
        public IActionResult Create()
        {
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "Name");
            return View();
        }

        // POST: EmployeeServices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeID,ServiceID")] EmployeeService employeeService)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeeService);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "Id", "FullName", employeeService.EmployeeID);
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "Name", employeeService.ServiceID);
            return View(employeeService);
        }

        // GET: EmployeeServices/Edit/5
        public async Task<IActionResult> Edit(int? employeeId, int? serviceId)
        {
            if (employeeId == null || serviceId == null)
            {
                return NotFound();
            }

            var employeeService = await _context.EmployeeServices
                .FindAsync(employeeId, serviceId);

            if (employeeService == null)
            {
                return NotFound();
            }

            ViewData["EmployeeID"] = new SelectList(_context.Employees, "Id", "FullName", employeeService.EmployeeID);
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "Name", employeeService.ServiceID);
            return View(employeeService);
        }

        // POST: EmployeeServices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int employeeId, int serviceId, [Bind("EmployeeID,ServiceID")] EmployeeService employeeService)
        {
            if (employeeId != employeeService.EmployeeID || serviceId != employeeService.ServiceID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeService);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeServiceExists(employeeService.EmployeeID, employeeService.ServiceID))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "Id", "FullName", employeeService.EmployeeID);
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "Name", employeeService.ServiceID);
            return View(employeeService);
        }

        // GET: EmployeeServices/Delete/5
        public async Task<IActionResult> Delete(int? employeeId, int? serviceId)
        {
            if (employeeId == null || serviceId == null)
            {
                return NotFound();
            }

            var employeeService = await _context.EmployeeServices
                .Include(e => e.Employee)
                .Include(e => e.Service)
                .FirstOrDefaultAsync(m => m.EmployeeID == employeeId && m.ServiceID == serviceId);

            return employeeService == null ? NotFound() : View(employeeService);
        }

        // POST: EmployeeServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int employeeId, int serviceId)
        {
            var employeeService = await _context.EmployeeServices
                .FindAsync(employeeId, serviceId);

            if (employeeService != null)
            {
                _context.EmployeeServices.Remove(employeeService);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeServiceExists(int employeeId, int serviceId)
        {
            return _context.EmployeeServices
                .Any(e => e.EmployeeID == employeeId && e.ServiceID == serviceId);
        }
    }
}