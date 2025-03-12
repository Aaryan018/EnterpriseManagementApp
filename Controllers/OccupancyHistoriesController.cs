using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;
using EnterpriseManagementApp;
//using EnterpriseManagementApp.DTO;
using EnterpriseManagementApp.Models;
using EnterpriseManagementApp.Data;

namespace EnterpriseManagementApp.Controllers
{
    public class OccupancyHistoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OccupancyHistoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OccupancyHistories
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.OccupancyHistories.Include(o => o.Asset).Include(o => o.Customer);
            return View(await applicationDbContext.ToListAsync());
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
                .FirstOrDefaultAsync(m => m.OccupancyHistoryId == id);
            if (occupancyHistory == null)
            {
                return NotFound();
            }

            return View(occupancyHistory);
        }

        // GET: OccupancyHistories/Create
        public IActionResult Create()
        {
            ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "Name");
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FullName");
            return View();
        }

        // POST: OccupancyHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,AssetId,Start,End,Paid,AmmountDue")] OccupancyHistory occupancyHistory)
        {
            // Check if the model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the related Customer and Asset entities from the database
                    occupancyHistory.Customer = await _context.Customers
                        .FirstOrDefaultAsync(c => c.Id == occupancyHistory.CustomerId);

                    occupancyHistory.Asset = await _context.Assets
                        .FirstOrDefaultAsync(a => a.AssetId == occupancyHistory.AssetId);

                    Debug.WriteLine("OH create - CP1");
                    Debug.WriteLine("Cus: " + occupancyHistory.Customer);
                    Debug.WriteLine("CusType: " + occupancyHistory.Customer.GetType());

                    Debug.WriteLine("As: " + occupancyHistory.Asset);
                    Debug.WriteLine("AsType: " + occupancyHistory.Asset.GetType());

                    if (occupancyHistory.Customer == null || occupancyHistory.Asset == null)
                    {
                        return Json(new { success = false, message = "Customer or Asset not found." });
                    }

                    occupancyHistory.OccupancyHistoryId = Guid.NewGuid();
                    occupancyHistory.Status = "Pending";

                    Debug.WriteLine("OH - CP2 - Check properties");
                    Debug.WriteLine("OCID: " + occupancyHistory.OccupancyHistoryId);
                    Debug.WriteLine("CusId: " + occupancyHistory.CustomerId);
                    Debug.WriteLine("AssetId: " + occupancyHistory.AssetId);
                    Debug.WriteLine("Cus: " + occupancyHistory.Customer);
                    Debug.WriteLine("Asset: " + occupancyHistory.Asset);
                    Debug.WriteLine("start: " + occupancyHistory.Start);
                    Debug.WriteLine("end: " + occupancyHistory.End);
                    Debug.WriteLine("paid: " + occupancyHistory.Paid);
                    Debug.WriteLine("ammount due: " + occupancyHistory.AmmountDue);
                    Debug.WriteLine("status: " + occupancyHistory.Status);

                    //// Add to the context and save changes
                    //_context.Add(occupancyHistory);
                    //await _context.SaveChangesAsync();

                    try
                    {
                        // Add to the context and save changes
                        _context.Add(occupancyHistory);
                        await _context.SaveChangesAsync();

                        // If everything is successful, you can return a success message as JSON.
                        return Json(new { success = true, message = "Occupancy history created successfully." });
                    }
                    catch (DbUpdateException dbEx)
                    {
                        // Log the database-specific exception (optional)
                        Debug.WriteLine($"Database error: {dbEx.Message}");
                        // Optionally, check for inner exceptions if there are any
                        if (dbEx.InnerException != null)
                        {
                            Debug.WriteLine($"Inner exception: {dbEx.InnerException.Message}");
                        }
                        // Return a JSON response with the error details
                        return Json(new { success = false, message = "Database error occurred while saving the data. Please try again later.", error = dbEx.Message });
                    }
                    catch (Exception ex)
                    {
                        // Log the general exception (optional)
                        Debug.WriteLine($"General error: {ex.Message}");

                        // Return a JSON response with the error details
                        return Json(new { success = false, message = "An unexpected error occurred. Please try again later.", error = ex.Message });
                    }



                    Debug.WriteLine("OH create - CP3. Context updated");

                    var occupancyHistoryDTO = new OccupancyHistoryDTO
                    {
                        OccupancyHistoryId = occupancyHistory.OccupancyHistoryId,
                        CustomerId = occupancyHistory.CustomerId,
                        AssetId = occupancyHistory.AssetId,
                        Start = occupancyHistory.Start,
                        End = occupancyHistory.End,
                        Paid = occupancyHistory.Paid,
                        AmmountDue = occupancyHistory.AmmountDue,
                        Status = occupancyHistory.Status
                    };

                    // Return a success response with the created occupancy history details
                    return Json(new { success = true, message = "Occupancy history created successfully.", data = occupancyHistoryDTO });
                }
                catch (Exception ex)
                {
                    // Return error if there was an exception
                    return Json(new { success = false, message = "An error occurred while creating the occupancy history.", error = ex.Message });
                }
            }

            // If the model is invalid, return validation errors
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { field = x.Key, errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                .ToList();

            return Json(new { success = false, message = "Validation failed.", errors });
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("CustomerId,AssetId,Start,End,Paid,AmmountDue")] OccupancyHistory occupancyHistory)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Get the related Customer and Asset entities from the database
        //        occupancyHistory.Customer = await _context.Customers
        //            .FirstOrDefaultAsync(c => c.CustomerId == occupancyHistory.CustomerId);

        //        occupancyHistory.Asset = await _context.Assets
        //            .FirstOrDefaultAsync(a => a.AssetId == occupancyHistory.AssetId);


        //        occupancyHistory.OccupancyHistoryId = Guid.NewGuid();
        //        occupancyHistory.Status = "pending";
        //        _context.Add(occupancyHistory);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    // If the model is not valid, repopulate the dropdowns and return the view
        //    ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "Address", occupancyHistory.AssetId);
        //    ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Address", occupancyHistory.CustomerId);
        //    return View(occupancyHistory);
        //}

        // GET: OccupancyHistories/Edit/5
        public async Task<IActionResult> Edit(string CustomerId, Guid? AssetId)
        {
            if (CustomerId == null || AssetId == null)
            {
                return NotFound();
            }

            //var occupancyHistory = await _context.OccupancyHistories.FindAsync(id);

            var occupancyHistory = await _context.OccupancyHistories
                .FirstOrDefaultAsync(o => o.CustomerId == CustomerId && o.AssetId == AssetId);


            if (occupancyHistory == null)
            {
                return NotFound();
            }
            ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "Address", occupancyHistory.AssetId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Address", occupancyHistory.CustomerId);
            return View(occupancyHistory);
        }

        // POST: OccupancyHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("OccupancyHistoryId,CustomerId,AssetId,Start,End,Paid,AmmountDue,Status")] OccupancyHistory occupancyHistory)
        {
            //if (id != occupancyHistory.OccupancyHistoryId)
            //{
            //    Debug.WriteLine("id: " + id);
            //    Debug.WriteLine("CustomerId: " + occupancyHistory.CustomerId);
            //    Debug.WriteLine("CP0 - id not found");
            //    return NotFound();
            //}

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(occupancyHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OccupancyHistoryExists(occupancyHistory.OccupancyHistoryId))
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
            ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "Address", occupancyHistory.AssetId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Address", occupancyHistory.CustomerId);
            Debug.WriteLine("CP - Model State Invalid");
            // Log the values of the incoming model for debugging
            Debug.WriteLine("OCID: " + occupancyHistory.OccupancyHistoryId);
            Debug.WriteLine("CusId: " + occupancyHistory.CustomerId);
            Debug.WriteLine("AssetId: " + occupancyHistory.AssetId);
            Debug.WriteLine("start: " + occupancyHistory.Start);
            Debug.WriteLine("end: " + occupancyHistory.End);
            Debug.WriteLine("paid: " + occupancyHistory.Paid);
            Debug.WriteLine("ammount due: " + occupancyHistory.AmmountDue);
            Debug.WriteLine("status: " + occupancyHistory.Status);
            return View(occupancyHistory);
        }

        // GET: OccupancyHistories/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Debug.WriteLine(id);

            var occupancyHistory = await _context.OccupancyHistories
                .Include(o => o.Asset)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OccupancyHistoryId == id);
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
            Debug.WriteLine("id [passed to delete post]: " + id);

            var occupancyHistory = await _context.OccupancyHistories
                .FirstOrDefaultAsync(o => o.OccupancyHistoryId == id);

            if (occupancyHistory != null)
            {
                _context.OccupancyHistories.Remove(occupancyHistory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool OccupancyHistoryExists(Guid id)
        {
            return _context.OccupancyHistories.Any(e => Guid.Parse(e.CustomerId) == id);
        }
    }
}
