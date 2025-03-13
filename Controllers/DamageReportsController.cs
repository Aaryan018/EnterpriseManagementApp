using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnterpriseManagementApp.Data;
using EnterpriseManagementApp.Models.Rentals;

namespace EnterpriseManagementApp.Controllers
{
    public class DamageReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DamageReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DamageReports
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DamageReports.Include(d => d.Asset);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DamageReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var damageReport = await _context.DamageReports
                .Include(d => d.Asset)
                .FirstOrDefaultAsync(m => m.DamageID == id);
            if (damageReport == null)
            {
                return NotFound();
            }

            return View(damageReport);
        }

        // GET: DamageReports/Create
        public IActionResult Create()
        {
            ViewData["AssetID"] = new SelectList(_context.Assets, "AssetID", "Status");
            return View();
        }

        // POST: DamageReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DamageID,AssetID,ReportDate,Description,RepairStatus")] DamageReport damageReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(damageReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssetID"] = new SelectList(_context.Assets, "AssetID", "Status", damageReport.AssetID);
            return View(damageReport);
        }

        // GET: DamageReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var damageReport = await _context.DamageReports.FindAsync(id);
            if (damageReport == null)
            {
                return NotFound();
            }
            ViewData["AssetID"] = new SelectList(_context.Assets, "AssetID", "Status", damageReport.AssetID);
            return View(damageReport);
        }

        // POST: DamageReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DamageID,AssetID,ReportDate,Description,RepairStatus")] DamageReport damageReport)
        {
            if (id != damageReport.DamageID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(damageReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DamageReportExists(damageReport.DamageID))
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
            ViewData["AssetID"] = new SelectList(_context.Assets, "AssetID", "Status", damageReport.AssetID);
            return View(damageReport);
        }

        // GET: DamageReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var damageReport = await _context.DamageReports
                .Include(d => d.Asset)
                .FirstOrDefaultAsync(m => m.DamageID == id);
            if (damageReport == null)
            {
                return NotFound();
            }

            return View(damageReport);
        }

        // POST: DamageReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var damageReport = await _context.DamageReports.FindAsync(id);
            if (damageReport != null)
            {
                _context.DamageReports.Remove(damageReport);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DamageReportExists(int id)
        {
            return _context.DamageReports.Any(e => e.DamageID == id);
        }
    }
}
