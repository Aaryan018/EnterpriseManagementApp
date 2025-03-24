using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnterpriseManagementApp;

namespace EnterpriseManagementApp.Controllers
{
    public class PayrollsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PayrollsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Payrolls
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Payrolls.Include(p => p.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Payrolls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payrolls
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.PayrollId == id);
            if (payroll == null)
            {
                return NotFound();
            }

            return View(payroll);
        }

        // Method to calculate payroll details
        private void CalculatePayroll(Payroll payroll, DateTime clockedInTime, DateTime clockedOutTime)
        {
            // Regular Hours: Static 8 hours from 9 AM to 5 PM
            payroll.RegularHours = 8;

            // Overtime: Calculate if clocked out after 5 PM
            payroll.OvertimeHours = (clockedOutTime.TimeOfDay > new TimeSpan(17, 0, 0)) ?
                                    (clockedOutTime - clockedOutTime.Date.AddHours(17)).TotalHours : 0;

            // Late Time: Calculate if clocked in after 9 AM
            payroll.LateTimeHours = (clockedInTime.TimeOfDay > new TimeSpan(9, 0, 0)) ?
                                    (clockedInTime - clockedInTime.Date.AddHours(9)).TotalHours : 0;

            // Total Pay Calculation: Regular Pay + Overtime Pay + Late Time Pay
            payroll.TotalPay += (payroll.RegularHours * payroll.HourlyRate) +
                               (payroll.OvertimeHours * payroll.HourlyRate * 1.5) +
                               (payroll.LateTimeHours * payroll.HourlyRate);
        }

        // GET: Payrolls/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Id");
            return View();
        }

        // POST: Payrolls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PayrollId,EmployeeId,PayrollGenerate,TotalPay,RegularHours,OvertimeHours,LateTimeHours,HourlyRate")] Payroll payroll)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the employee's hourly rate
                var employee = await _context.Employees.FindAsync(payroll.EmployeeId);

                if (employee != null)
                {
                    payroll.HourlyRate = (double)employee.HourlyRate;

                    // Fetch the Attendance records for the given EmployeeId
                    var attendance = await _context.Attendances
                        .Where(a => a.EmployeeId == payroll.EmployeeId)
                        .ToListAsync();

                    if (attendance.Count == 0)
                    {
                        ModelState.AddModelError("", "No attendance records found for this period.");
                        return View(payroll);
                    }

                    // Loop through each attendance record to calculate the payroll
                    foreach (var record in attendance)
                    {
                        // Calculate payroll details (regular, overtime, and late hours)
                        if (record.ClockedInTime != null && record.ClockedOutTime != null)
                        {
                            // Calculate payroll details (regular, overtime, and late hours)
                            CalculatePayroll(payroll, record.ClockedInTime, record.ClockedOutTime);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Attendance record does not contain clock-in or clock-out times.");
                            return View(payroll);
                        }

                    }

                    // Save the payroll
                    _context.Add(payroll);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Employee not found.");
                    return View(payroll);
                }
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Id", payroll.EmployeeId);
            return View(payroll);
        }

        // GET: Payrolls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payrolls
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.PayrollId == id);
            if (payroll == null)
            {
                return NotFound();
            }
            
            return View(payroll);
        }

        // POST: Payrolls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payroll = await _context.Payrolls.FindAsync(id);
            if (payroll != null)
            {
                _context.Payrolls.Remove(payroll);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PayrollExists(int id)
        {
            return _context.Payrolls.Any(e => e.PayrollId == id);
        }
    }
}