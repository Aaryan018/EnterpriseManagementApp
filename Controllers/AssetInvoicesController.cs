using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnterpriseManagementApp;
using EnterpriseManagementApp.Models.Rentals;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Diagnostics;
using EnterpriseManagementApp.Models;
using NuGet.ContentModel;

namespace EnterpriseManagementApp.Controllers
{
    [Authorize(Roles = "Customer")]
    public class AssetInvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssetInvoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AssetInvoices
        public async Task<IActionResult> Index()
        {
            // Get the logged-in user's CustomerId (assuming the claim type is 'sub' or 'NameIdentifier')
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(); // In case the user is not logged in properly
            }

            // Fetch the AssetInvoices related to the logged-in user (Customer)
            var assetInvoices = await _context.AssetInvoices
                .Where(ai => ai.CustomerId == currentUserId) // Filter by CustomerId
                .Include(ai => ai.OccupancyHistory) // Include related Asset data, if needed
                .ToListAsync();

            return View(assetInvoices);
        }

        // GET: AssetInvoices/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetInvoice = await _context.AssetInvoices
                .Include(a => a.OccupancyHistory)
                .FirstOrDefaultAsync(m => m.AssetInvoiceId == id);
            if (assetInvoice == null)
            {
                return NotFound();
            }

            return View(assetInvoice);
        }

        // GET: AssetInvoices/Create
        public IActionResult Create(Guid? occupancyHistoryId)
        {
            //ViewData["CustomerId"] = new SelectList(_context.OccupancyHistories, "CustomerId", "CustomerId");
            //return View();

            if (occupancyHistoryId == null)
            {
                return NotFound(); // You can handle this case if the id isn't provided or invalid.
            }

            // You can retrieve the related OccupancyHistory or use this ID as needed.
            var occupancyHistory = _context.OccupancyHistories
                .FirstOrDefault(o => o.OccupancyHistoryId == occupancyHistoryId);

            if (occupancyHistory == null)
            {
                return NotFound(); // Handle case where the occupancy history is not found
            }

            ViewData["CustomerId"] = occupancyHistory.CustomerId;
            ViewData["AssetId"] = occupancyHistory.AssetId;


            //// You can now use occupancyHistory to pre-fill the form or pass any related data.
            //ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", occupancyHistory.CustomerId);

            //// Optionally, pass the occupancyHistoryId or occupancyHistory to the view if you need it
            //ViewData["OccupancyHistoryId"] = occupancyHistoryId;

            return View();

        }

        // POST: AssetInvoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,AssetId,DatePaid,AmmountPaid")] AssetInvoice assetInvoice)
        {
            if (ModelState.IsValid)
            {
                Debug.WriteLine("AI create - CP1");
                assetInvoice.AssetInvoiceId = Guid.NewGuid();
                assetInvoice.OccupancyHistory = await _context.OccupancyHistories
                    .Include(o => o.Asset)
                    .Include(o => o.Customer)
                    .FirstOrDefaultAsync(m => m.AssetId == assetInvoice.AssetId && m.CustomerId == assetInvoice.CustomerId);

                if (assetInvoice.OccupancyHistory == null)
                {
                    Debug.WriteLine("CP 2 - given Asset and Customer id's could not find associated OH");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Success !!
                    Debug.WriteLine("CP 2- OH id is : " + assetInvoice.OccupancyHistory.OccupancyHistoryId);

                    // Update the 'Paid' property of the OccupancyHistory record
                    assetInvoice.OccupancyHistory.Paid += assetInvoice.AmmountPaid; // Add the amount paid to the existing 'Paid' value

                    // Mark the OccupancyHistory record as modified in the context
                    _context.OccupancyHistories.Update(assetInvoice.OccupancyHistory);

                    _context.Add(assetInvoice);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                
            } else
            {
                Debug.WriteLine("AI create - CP3 -- Model State is Invalid");
                // If the model is invalid, return validation errors
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { field = x.Key, errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                    .ToList();
                Debug.WriteLine("AssetInvoice ID: " + assetInvoice.AssetInvoiceId);
                Debug.WriteLine("CusId: " + assetInvoice.CustomerId);
                Debug.WriteLine("assetId: " + assetInvoice.AssetId);
                Debug.WriteLine("ammount Paid: " + assetInvoice.AmmountPaid);
                Debug.WriteLine("Date Paid: " + assetInvoice.DatePaid);
                //Debug.WriteLine("OHID: " + assetInvoice.OccupancyHistory);
                //ViewData["CustomerId"] = new SelectList(_context.OccupancyHistories, "CustomerId", "CustomerId", assetInvoice.CustomerId);
                return View(assetInvoice);
            }

        }

        // GET: AssetInvoices/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetInvoice = await _context.AssetInvoices.FindAsync(id);
            if (assetInvoice == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.OccupancyHistories, "CustomerId", "CustomerId", assetInvoice.CustomerId);
            return View(assetInvoice);
        }

        // POST: AssetInvoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("AssetInvoiceId,CustomerId,AssetId,DatePaid,AmmountPaid")] AssetInvoice assetInvoice)
        {
            if (id != assetInvoice.AssetInvoiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assetInvoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssetInvoiceExists(assetInvoice.AssetInvoiceId))
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
            ViewData["CustomerId"] = new SelectList(_context.OccupancyHistories, "CustomerId", "CustomerId", assetInvoice.CustomerId);
            return View(assetInvoice);
        }

        // GET: AssetInvoices/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetInvoice = await _context.AssetInvoices
                .Include(a => a.OccupancyHistory)
                .FirstOrDefaultAsync(m => m.AssetInvoiceId == id);
            if (assetInvoice == null)
            {
                return NotFound();
            }

            return View(assetInvoice);
        }

        // POST: AssetInvoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var assetInvoice = await _context.AssetInvoices.FindAsync(id);
            if (assetInvoice != null)
            {
                _context.AssetInvoices.Remove(assetInvoice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssetInvoiceExists(Guid id)
        {
            return _context.AssetInvoices.Any(e => e.AssetInvoiceId == id);
        }
    }
}
