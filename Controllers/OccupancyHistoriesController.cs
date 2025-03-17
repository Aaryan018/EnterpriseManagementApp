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
using EnterpriseManagementApp.DTO;
using EnterpriseManagementApp.Models;

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
            // Retrieve the records, ordered by status
            var occupancyHistories = await _context.OccupancyHistories
                .Include(o => o.Asset)
                .Include(o => o.Customer)
                .OrderBy(oh => oh.Status == "Pending" ? 0 :
                              oh.Status == "Approved" ? 1 : 2)  // Custom order
                .ThenBy(oh => oh.Status)  // Optional: if you want to break ties (e.g., alphabetically by Asset/Customer)
                .ToListAsync();  // Load the data into memory

            // Loop through the records and check/update properties as needed
            foreach (var occupancyHistory in occupancyHistories)
            {
                // Access the related Asset data
                var asset = occupancyHistory.Asset;

                // Update each occupancyHistory's Ammount due and Next payment values ...
                // Ammount due is Asset.RentRate * 'how many months there are between start and end date of occupancyHistry record
                if (asset != null)
                {
                    // ... 
                    // DateOnly today = DateOnly.FromDateTime(DateTime.Today); 

                    DateOnly startDate = occupancyHistory.Start;
                    DateOnly endDate = occupancyHistory.End;

                    // Calculate the months difference between Start and End dates
                    int monthsDifference = endDate.Month - startDate.Month + 12 * (endDate.Year - startDate.Year);

                    // Adjust months difference if the End day is before the Start day in the month
                    if (endDate.Day < startDate.Day)
                    {
                        monthsDifference--; // Subtract one month if the End date is before the Start date in the month
                    }

                    occupancyHistory.TotalDue = monthsDifference * asset.RentRate;

                    occupancyHistory.RemainingBalance = occupancyHistory.TotalDue - occupancyHistory.Paid;

                    // Mark the entity as modified
                    _context.OccupancyHistories.Update(occupancyHistory);
                }
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Pass the modified records to the view
            return View(occupancyHistories);

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
                    occupancyHistory.TotalDue = occupancyHistory.Asset.RentRate;

                    Debug.WriteLine("OH - CP2 - Check properties");
                    Debug.WriteLine("OCID: " + occupancyHistory.OccupancyHistoryId);
                    Debug.WriteLine("CusId: " + occupancyHistory.CustomerId);
                    Debug.WriteLine("AssetId: " + occupancyHistory.AssetId);
                    Debug.WriteLine("Cus: " + occupancyHistory.Customer);
                    Debug.WriteLine("Asset: " + occupancyHistory.Asset);
                    Debug.WriteLine("start: " + occupancyHistory.Start);
                    Debug.WriteLine("end: " + occupancyHistory.End);
                    Debug.WriteLine("paid: " + occupancyHistory.Paid);
                    Debug.WriteLine("total due: " + occupancyHistory.TotalDue);
                    Debug.WriteLine("status: " + occupancyHistory.Status);

                    try
                    {
                        // Add to the context and save changes
                        _context.Add(occupancyHistory);
                        await _context.SaveChangesAsync();

                        // If everything is successful, you can return a success message as JSON.
                        //return Json(new { success = true, message = "Occupancy history created successfully." });
                        return RedirectToAction(nameof(Index));
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

                        ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FullName", occupancyHistory.CustomerId);

                        // Exclude AssetIds that are already in the OccupancyHistory for the selected CustomerId
                        var occupiedAssets = _context.OccupancyHistories
                                                    .Where(oh => oh.CustomerId == occupancyHistory.CustomerId)
                                                    .Select(oh => oh.AssetId)
                                                    .ToList();
                        var availableAssets = _context.Assets
                                                      .Where(a => !occupiedAssets.Contains(a.AssetId))
                                                      .Select(a => new { a.AssetId, a.Name })
                                                      .ToList();

                        // If some alternative assets available, leave message
                        if (availableAssets.Count > 0)
                        {
                            // Set a message to inform the user
                            ViewData["Message"] = "Previously selected Asset was already leased to Renter. Please select alternate option from newly provided options.";
                        }

                        // If no available assets, add all assets to the ViewData and set a message
                        if (availableAssets.Count == 0)
                        {
                            // Get all assets (in case there are no available ones)
                            availableAssets = _context.Assets
                                                      .Select(a => new { a.AssetId, a.Name })
                                                      .ToList();

                            // Set a message to inform the user
                            ViewData["Message"] = "No available assets for the selected customer.";
                        }

                        ViewData["AssetId"] = new SelectList(availableAssets, "AssetId", "Name");

                        return View(occupancyHistory);

                        // Return a JSON response with the error details
                        // return Json(new { success = false, message = "Database error occurred while saving the data. Please try again later.", error = dbEx.Message });
                    }
                    catch (Exception ex)
                    {
                        // Log the general exception (optional)
                        Debug.WriteLine($"General error: {ex.Message}");

                        // Return a JSON response with the error details
                        return Json(new { success = false, message = "An unexpected error occurred. Please try again later.", error = ex.Message });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Geneal exception");
                    // Return error if there was an exception
                    return Json(new { success = false, message = "An error occurred while creating the occupancy history.", error = ex.Message });
                }
            }

            // If the model is invalid, return validation errors
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { field = x.Key, errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                .ToList();

            Debug.WriteLine("CP4 - errors:" + errors);
            Debug.WriteLine("CP4 - Check properties");
            Debug.WriteLine("OCID: " + occupancyHistory.OccupancyHistoryId);
            Debug.WriteLine("CusId: " + occupancyHistory.CustomerId);
            Debug.WriteLine("AssetId: " + occupancyHistory.AssetId);
            Debug.WriteLine("Cus: " + occupancyHistory.Customer);
            Debug.WriteLine("Asset: " + occupancyHistory.Asset);
            Debug.WriteLine("start: " + occupancyHistory.Start);
            Debug.WriteLine("end: " + occupancyHistory.End);
            Debug.WriteLine("paid: " + occupancyHistory.Paid);
            Debug.WriteLine("total due: " + occupancyHistory.TotalDue);
            Debug.WriteLine("status: " + occupancyHistory.Status);

            //return Json(new { success = false, message = "Validation failed.", errors });
            return View(occupancyHistory);
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
            Debug.WriteLine("total due: " + occupancyHistory.TotalDue);
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

        // GET: OccupancyHistories/GetAvailableAssets
        public JsonResult GetAvailableAssets(string customerId)
        {
            // Get the occupied AssetIds for the given CustomerId
            var occupiedAssets = _context.OccupancyHistories
                                        .Where(oh => oh.CustomerId == customerId)
                                        .Select(oh => oh.AssetId)
                                        .ToList();

            // Get the available assets by excluding the occupied ones
            var availableAssets = _context.Assets
                                          .Where(a => !occupiedAssets.Contains(a.AssetId))
                                          .Select(a => new { a.AssetId, a.Name })
                                          .ToList();

            return Json(availableAssets);
        }
    }
}
