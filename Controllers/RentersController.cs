using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnterpriseManagementApp.Data;
using EnterpriseManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace EnterpriseManagementApp.Controllers
{
    [Authorize(Roles = "Manager")] // Apply role-based authorization at the controller level
    public class RentersController : Controller
    {
        private readonly ManageHousingDbContext _context;

        public RentersController(ManageHousingDbContext context)
        {
            _context = context;
        }

        // GET: Renters
        public async Task<IActionResult> Index()
        {
            return View(await _context.Renters.ToListAsync());
        }

        // GET: Renters/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var renter = await _context.Renters
                .FirstOrDefaultAsync(m => m.RenterId == id);
            if (renter == null)
            {
                return NotFound();
            }

            return View(renter);
        }

        // GET: Renters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Renters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RenterId,Name,Email,ContactNumber,Address")] Renter renter)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Ensure new RenterId for new entries
                    renter.RenterId = Guid.NewGuid();

                    _context.Add(renter);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the error for debugging
                    Debug.WriteLine($"Error saving data: {ex.Message}");
                    // Return the view with validation errors
                    return View(renter);
                }
            }
            return View(renter);
        }

        // GET: Renters/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var renter = await _context.Renters.FindAsync(id);
            if (renter == null)
            {
                return NotFound();
            }
            return View(renter);
        }

        // POST: Renters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("RenterId,Name,Email,ContactNumber,Address")] Renter renter)
        {
            if (id != renter.RenterId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(renter);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RenterExists(renter.RenterId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // Log the concurrency exception for debugging
                        Debug.WriteLine("Concurrency error occurred while updating renter.");
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    // Log the error for debugging
                    Debug.WriteLine($"Error updating data: {ex.Message}");
                    return View(renter);
                }
            }
            return View(renter);
        }

        // GET: Renters/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var renter = await _context.Renters
                .FirstOrDefaultAsync(m => m.RenterId == id);
            if (renter == null)
            {
                return NotFound();
            }

            return View(renter);
        }

        // POST: Renters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var renter = await _context.Renters.FindAsync(id);
            if (renter != null)
            {
                _context.Renters.Remove(renter);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RenterExists(Guid id)
        {
            return _context.Renters.Any(e => e.RenterId == id);
        }
    }
}
