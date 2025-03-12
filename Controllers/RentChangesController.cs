using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnterpriseManagementApp;
using EnterpriseManagementApp.Models;

namespace EnterpriseManagementApp.Controllers
{
    public class RentChangesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RentChangesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RentChanges
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.RentChanges.Include(r => r.Asset);
            return View(await applicationDbContext.ToListAsync());
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
                .FirstOrDefaultAsync(m => m.RentChangeId == id);
            if (rentChange == null)
            {
                return NotFound();
            }

            return View(rentChange);
        }

        // GET: RentChanges/Create
        public IActionResult Create()
        {
            ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "Address");
            return View();
        }

        // POST: RentChanges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RentChangeId,AssetId,ChangeDate,OldRate")] RentChange rentChange)
        {
            if (ModelState.IsValid)
            {
                rentChange.RentChangeId = Guid.NewGuid();
                _context.Add(rentChange);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "Address", rentChange.AssetId);
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
            ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "Address", rentChange.AssetId);
            return View(rentChange);
        }

        // POST: RentChanges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("RentChangeId,AssetId,ChangeDate,OldRate")] RentChange rentChange)
        {
            if (id != rentChange.RentChangeId)
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
                    if (!RentChangeExists(rentChange.RentChangeId))
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
            ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "Address", rentChange.AssetId);
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
                .FirstOrDefaultAsync(m => m.RentChangeId == id);
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
            return _context.RentChanges.Any(e => e.RentChangeId == id);
        }
    }
}
