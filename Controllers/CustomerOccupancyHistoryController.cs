using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnterpriseManagementApp;
using EnterpriseManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Diagnostics;

namespace EnterpriseManagementApp.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerOccupancyHistoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerOccupancyHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CustomerOccupancyHistory
        public async Task<IActionResult> Index()
        {
            // Get the logged-in user's CustomerId (assuming the claim type is 'sub' or 'NameIdentifier')
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(); // In case the user is not logged in properly
            }

            // Retrieve the records, ordered by status
            var occupancyHistories = await _context.OccupancyHistories
                .Where(o => o.CustomerId == currentUserId) // Filter by CustomerId
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

            return View(occupancyHistories);
        }

        //// GET: CustomerOccupancyHistory/Details/5
        //public async Task<IActionResult> Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var occupancyHistory = await _context.OccupancyHistories
        //        .Include(o => o.Asset)
        //        .Include(o => o.Customer)
        //        .FirstOrDefaultAsync(m => m.CustomerId == id);
        //    if (occupancyHistory == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(occupancyHistory);
        //}

        // GET: CustomerOccupancyHistory/Create
        public IActionResult Create()
        {
            // Check if the user is in the Customer role
            if (User.IsInRole("Customer"))
            {
                // Set the CustomerId to the logged-in user's CustomerId
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Fetch the CustomerId from the database for the logged-in user
                var customer = _context.Customers.FirstOrDefault(c => c.Id == currentUserId); // Assuming "UserId" is used to map to the User in your DB

                if (currentUserId != null)
                {
                    ViewData["CustomerId"] = currentUserId;
                }
                //if (customer != null)
                //{
                //    ViewData["CustomerId"] = customer.Id;
                //} else
                //{
                //    ViewData["CustomerId"] = "fails";
                //}
            }


            ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "Name");
            return View();
        }

        // POST: CustomerOccupancyHistory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,AssetId,Start,End,Paid,TotalDue")] OccupancyHistory occupancyHistory)
        {
            //if (ModelState.IsValid)
            //{
            //    _context.Add(occupancyHistory);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "Address", occupancyHistory.AssetId);
            //ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", occupancyHistory.CustomerId);
            //return View(occupancyHistory);

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
                    Debug.WriteLine("CustomerId binded: " + occupancyHistory.CustomerId);
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

                        occupancyHistory.TotalDue = monthsDifference * occupancyHistory.Asset.RentRate;

                        occupancyHistory.RemainingBalance = occupancyHistory.TotalDue;

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

                        ViewData["CustomerId"] = occupancyHistory.CustomerId;

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

        //// GET: CustomerOccupancyHistory/Edit/5
        //public async Task<IActionResult> Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var occupancyHistory = await _context.OccupancyHistories.FindAsync(id);
        //    if (occupancyHistory == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "Address", occupancyHistory.AssetId);
        //    ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", occupancyHistory.CustomerId);
        //    return View(occupancyHistory);
        //}

        //// POST: CustomerOccupancyHistory/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(string id, [Bind("OccupancyHistoryId,CustomerId,AssetId,Start,End,Paid,TotalDue,RemainingBalance,Status")] OccupancyHistory occupancyHistory)
        //{
        //    if (id != occupancyHistory.CustomerId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(occupancyHistory);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!OccupancyHistoryExists(occupancyHistory.CustomerId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["AssetId"] = new SelectList(_context.Assets, "AssetId", "Address", occupancyHistory.AssetId);
        //    ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", occupancyHistory.CustomerId);
        //    return View(occupancyHistory);
        //}

        //// GET: CustomerOccupancyHistory/Delete/5
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var occupancyHistory = await _context.OccupancyHistories
        //        .Include(o => o.Asset)
        //        .Include(o => o.Customer)
        //        .FirstOrDefaultAsync(m => m.CustomerId == id);
        //    if (occupancyHistory == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(occupancyHistory);
        //}

        //// POST: CustomerOccupancyHistory/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    var occupancyHistory = await _context.OccupancyHistories.FindAsync(id);
        //    if (occupancyHistory != null)
        //    {
        //        _context.OccupancyHistories.Remove(occupancyHistory);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool OccupancyHistoryExists(string id)
        {
            return _context.OccupancyHistories.Any(e => e.CustomerId == id);
        }
    }
}
