using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnterpriseManagementApp.Data;
using EnterpriseManagementApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace EnterpriseManagementApp.Controllers
{
    [Authorize(Roles = "Manager")] // Apply role-based authorization at the controller level
    public class RentChangesController : Controller
    {
        private readonly ManageHousingDbContext _context;

        public RentChangesController(ManageHousingDbContext context)
        {
            _context = context;
        }

        // GET: RentChanges
        public async Task<IActionResult> Index()
        {
            var manageHousingDbContext = _context.RentChanges.Include(r => r.Asset);
            return View(await manageHousingDbContext.ToListAsync());
        }

        // GET: RentChanges/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rentChange = await _context.RentChanges
                .Include(r => r.Asset)
                .FirstOrDefaultAsync(m => m.HistoryRentId == id);
            if (rentChange == null)
            {
                return NotFound();
            }

            return View(rentChange);
        }

        // GET: RentChanges/Create
        public IActionResult Create()
        {
            ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "AssetId");
            return View();
        }

        // POST: RentChanges/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HistoryRentId,AssetId,RenterId,ChangeDate,OldRate,Status")] RentChange rentChange)
        {
            if (ModelState.IsValid)
            {
                rentChange.HistoryRentId = Guid.NewGuid();
                _context.Add(rentChange);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "AssetId", rentChange.AssetId);
            return View(rentChange);
        }

        // GET: RentChanges/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rentChange = await _context.RentChanges.FindAsync(id);
            if (rentChange == null)
            {
                return NotFound();
            }
            ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "AssetId", rentChange.AssetId);
            return View(rentChange);
        }

        // POST: RentChanges/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("HistoryRentId,AssetId,RenterId,ChangeDate,OldRate,Status")] RentChange rentChange)
        {
            if (id != rentChange.HistoryRentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rentChange);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentChangeExists(rentChange.HistoryRentId))
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
            ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "AssetId", rentChange.AssetId);
            return View(rentChange);
        }

        // GET: RentChanges/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rentChange = await _context.RentChanges
                .Include(r => r.Asset)
                .FirstOrDefaultAsync(m => m.HistoryRentId == id);
            if (rentChange == null)
            {
                return NotFound();
            }

            return View(rentChange);
        }

        // POST: RentChanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var rentChange = await _context.RentChanges.FindAsync(id);
            if (rentChange != null)
            {
                _context.RentChanges.Remove(rentChange);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RentChangeExists(Guid id)
        {
            return _context.RentChanges.Any(e => e.HistoryRentId == id);
        }
    }
}
