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
    public class AppEventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppEventController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET: AppEvent/Index
        public async Task<IActionResult> Index()
        {
            var userName = User?.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            ViewData["UserRole"] = user.Role;

            if (user == null)
            {
                return Unauthorized(); // Handle case where user is not found
            }

            IQueryable<AppEvent> eventsQuery = _context.AppEvent
                .Include(e => e.Service)   // Include Service details
                .Include(e => e.Customers); // Include Customers linked to the event

            if (user.Role == "Client")
            {
                // 1️⃣ Find the current logged-in customer's ID
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == user.Id);

                if (customer != null)
                {
                    // 2️⃣ Filter events to only those that include this customer
                    eventsQuery = eventsQuery.Where(e => e.Customers.Any(c => c.Id == customer.Id));
                }
                else
                {
                    // No customer entry found, return an empty list
                    return View(new List<AppEvent>());
                }
            }

            // 3️⃣ Retrieve filtered results
            var events = await eventsQuery.ToListAsync();
            return View(events);
        }

        // GET: AppEvent/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appEvent = await _context.AppEvent
                .Include(a => a.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appEvent == null)
            {
                return NotFound();
            }

            return View(appEvent);
        }
        
        // GET Create View
        public async Task<IActionResult> Create()
        {
            var userName = User?.Identity?.Name; // Assumes UserName matches ApplicationUser.UserName
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user?.Role == "Manager") // Ensure user is not null before checking role
            {
                // 1️⃣ Get all users with role "Client"
                var clients = await _context.Customers
                    .Select(u => new { u.Id, u.FullName }) 
                    .ToListAsync();

                // 2️⃣ Store clients in ViewData for dropdown
                ViewData["Customers"] = new SelectList(clients, "Id", "FullName");
            }
    
            ViewData["UserRole"] = user?.Role; 
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name");

            return View();
        }
        
        // POST: AppEvent/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ServiceId,StartTime,EndTime,Status")] AppEvent appEvent, string? CustomerId)
        {
            var userName = User?.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                return Unauthorized(); // Handle case where user is not found
            }

            List<Customer> eventCustomers = new List<Customer>();

            if (user.Role == "Client")
            {
                // 1️⃣ Find the currently logged-in customer's ID
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == user.Id);

                if (customer != null)
                {
                    eventCustomers.Add(customer); // Add the logged-in customer
                }
            }
            else if (user.Role == "Manager" && !string.IsNullOrEmpty(CustomerId))
            {
                // 2️⃣ Use the selected CustomerId from the form (dropdown)
                var selectedCustomer = await _context.Customers.FindAsync(CustomerId);

                if (selectedCustomer != null)
                {
                    eventCustomers.Add(selectedCustomer);
                }
            }

            if (ModelState.IsValid)
            {
                appEvent.Customers = eventCustomers; // Assign the customers to the event
                _context.Add(appEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // 🔹 If validation fails, repopulate dropdowns
            ViewData["UserRole"] = user.Role;
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appEvent.ServiceId);

            if (user.Role == "Manager")
            {
                var clients = await _context.Customers
                    .Select(c => new { c.Id, c.FullName })
                    .ToListAsync();

                ViewData["Customers"] = new SelectList(clients, "Id", "FullName", CustomerId);
            }

            return View(appEvent);
        }
        
        
        // GET: AppEvent/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appEvent = await _context.AppEvent.FindAsync(id);
            if (appEvent == null)
            {
                return NotFound();
            }
            var userName = User?.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            ViewData["UserRole"] = user.Role;
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appEvent.ServiceId);
            return View(appEvent);
        }

        // POST: AppEvent/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ServiceId,StartTime,EndTime,Status")] AppEvent appEvent)
        {
            if (id != appEvent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appEvent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppEventExists(appEvent.Id))
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
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Id", appEvent.ServiceId);
            return View(appEvent);
        }

        // GET: AppEvent/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appEvent = await _context.AppEvent
                .Include(a => a.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appEvent == null)
            {
                return NotFound();
            }

            return View(appEvent);
        }

        // POST: AppEvent/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appEvent = await _context.AppEvent.FindAsync(id);
            if (appEvent != null)
            {
                _context.AppEvent.Remove(appEvent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppEventExists(int id)
        {
            return _context.AppEvent.Any(e => e.Id == id);
        }
    }
}
