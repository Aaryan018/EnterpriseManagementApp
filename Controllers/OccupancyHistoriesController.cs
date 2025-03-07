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
    public class OccupancyHistoriesController : Controller
    {
        private readonly ManageHousingDbContext _context;

        public OccupancyHistoriesController(ManageHousingDbContext context)
        {
            _context = context;
        }

        // GET: OccupancyHistories
        public async Task<IActionResult> Index()
        {
            var manageHousingDbContext = _context.OccupancyHistories.Include(o => o.Asset).Include(o => o.Customer);
            return View(await manageHousingDbContext.ToListAsync());
        }

        // GET: OccupancyHistories/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var occupancyHistory = await _context.OccupancyHistories
                .Include(o => o.Asset)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OccupancyId == id);
            if (occupancyHistory == null)
            {
                return NotFound();
            }

            return View(occupancyHistory);
        }

        // GET: OccupancyHistories/Create
        public IActionResult Create()
        {
            ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "AssetId");
            ViewData["CustomerId"] = new SelectList(_context.Renters, "RenterId", "Email");
            return View();
        }

        // POST: OccupancyHistories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OccupancyId,RenterId,CustomerId,AssetId,Start,End,AmountDue,Status")] OccupancyHistory occupancyHistory)
        {
            if (ModelState.IsValid)
            {
                occupancyHistory.OccupancyId = Guid.NewGuid();
                _context.Add(occupancyHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "AssetId", occupancyHistory.AssetId);
            ViewData["CustomerId"] = new SelectList(_context.Renters, "RenterId", "Email", occupancyHistory.CustomerId);
            return View(occupancyHistory);
        }

        // GET: OccupancyHistories/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var occupancyHistory = await _context.OccupancyHistories.FindAsync(id);
            if (occupancyHistory == null)
            {
                return NotFound();
            }
            ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "AssetId", occupancyHistory.AssetId);
            ViewData["CustomerId"] = new SelectList(_context.Renters, "RenterId", "Email", occupancyHistory.CustomerId);
            return View(occupancyHistory);
        }

        // POST: OccupancyHistories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("OccupancyId,RenterId,CustomerId,AssetId,Start,End,AmountDue,Status")] OccupancyHistory occupancyHistory)
        {
            if (id != occupancyHistory.OccupancyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(occupancyHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OccupancyHistoryExists(occupancyHistory.OccupancyId))
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
            ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "AssetId", occupancyHistory.AssetId);
            ViewData["CustomerId"] = new SelectList(_context.Renters, "RenterId", "Email", occupancyHistory.CustomerId);
            return View(occupancyHistory);
        }

        // GET: OccupancyHistories/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var occupancyHistory = await _context.OccupancyHistories
                .Include(o => o.Asset)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OccupancyId == id);
            if (occupancyHistory == null)
            {
                return NotFound();
            }

            return View(occupancyHistory);
        }

        // POST: OccupancyHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var occupancyHistory = await _context.OccupancyHistories.FindAsync(id);
            if (occupancyHistory != null)
            {
                _context.OccupancyHistories.Remove(occupancyHistory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OccupancyHistoryExists(Guid id)
        {
            return _context.OccupancyHistories.Any(e => e.OccupancyId == id);
        }
    }
}
