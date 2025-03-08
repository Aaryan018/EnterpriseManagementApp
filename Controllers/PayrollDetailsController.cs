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
    public class PayrollDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PayrollDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PayrollDetails
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PayrollDetails.Include(p => p.Payroll);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PayrollDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payrollDetails = await _context.PayrollDetails
                .Include(p => p.Payroll)
                .FirstOrDefaultAsync(m => m.PayrollDetailID == id);
            if (payrollDetails == null)
            {
                return NotFound();
            }

            return View(payrollDetails);
        }

        // GET: PayrollDetails/Create
        public IActionResult Create()
        {
            ViewData["PayrollID"] = new SelectList(_context.Payrolls, "PayrollID", "PayrollID");
            return View();
        }

        // POST: PayrollDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PayrollDetailID,PayrollID,Description,Amount")] PayrollDetails payrollDetails)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payrollDetails);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PayrollID"] = new SelectList(_context.Payrolls, "PayrollID", "PayrollID", payrollDetails.PayrollID);
            return View(payrollDetails);
        }

        // GET: PayrollDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payrollDetails = await _context.PayrollDetails.FindAsync(id);
            if (payrollDetails == null)
            {
                return NotFound();
            }
            ViewData["PayrollID"] = new SelectList(_context.Payrolls, "PayrollID", "PayrollID", payrollDetails.PayrollID);
            return View(payrollDetails);
        }

        // POST: PayrollDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PayrollDetailID,PayrollID,Description,Amount")] PayrollDetails payrollDetails)
        {
            if (id != payrollDetails.PayrollDetailID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payrollDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PayrollDetailsExists(payrollDetails.PayrollDetailID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PayrollID"] = new SelectList(_context.Payrolls, "PayrollID", "PayrollID", payrollDetails.PayrollID);
            return View(payrollDetails);
        }

        // GET: PayrollDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payrollDetails = await _context.PayrollDetails
                .Include(p => p.Payroll)
                .FirstOrDefaultAsync(m => m.PayrollDetailID == id);
            if (payrollDetails == null)
            {
                return NotFound();
            }

            return View(payrollDetails);
        }

        // POST: PayrollDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payrollDetails = await _context.PayrollDetails.FindAsync(id);
            if (payrollDetails != null)
            {
                _context.PayrollDetails.Remove(payrollDetails);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PayrollDetailsExists(int id)
        {
            return _context.PayrollDetails.Any(e => e.PayrollDetailID == id);
        }
    }
}
