using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnterpriseManagementApp;

namespace EnterpriseManagementApp.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User?.Identity?.Name; 
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userId);

            if (user == null || user?.Role == "Client")
            {
                return Unauthorized();
            }

            ViewData["UserRole"] = user?.Role; 

            var applicationDbContext = _context.Attendances
                .Include(a => a.Employee)
                .Include(a => a.Event)
                .ThenInclude(e => e.Service);

            return View(await applicationDbContext.ToListAsync());
        }
        // GET: Attendance/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            {
                if (id == null)
                {
                    return NotFound();
                }

                var attendance = await _context.Attendances
                    .Include(a => a.Employee)
                    .Include(a => a.Event)
                    .FirstOrDefaultAsync(m => m.AttendanceId == id);
                if (attendance == null)
                {
                    return NotFound();
                }

                var userId = User?.Identity?.Name;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userId);
                if (user == null || (user.Role != "Manager" && user.Id != attendance.EmployeeId))
                {
                    return Unauthorized();
                }

                return View(attendance);
            }
        }

        // GET: Attendance/Create
        public async Task<IActionResult> Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
    
            var userId = User?.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userId); // ✅ Use await properly

            if (user == null || user.Role != "Manager") // ✅ Prevent null reference issue
            {
                return Unauthorized();
            }

            // ✅ Include Service to access the Service.Name property
            var events = await _context.AppEvent
                .Include(e => e.Service) // Load the related Service
                .Select(e => new 
                { 
                    Id = e.Id, 
                    DisplayText = $"{e.Service.Name}: ({e.StartTime:MMM-dd-yyyy HH:mm} - {e.EndTime:HH:mm})",
                })
                .ToListAsync(); // ✅ Use ToListAsync() for async execution

            ViewData["EventId"] = new SelectList(events, "Id", "DisplayText");

            return View();
        }

        // POST: Attendance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AttendanceId,EmployeeId,EventId,ClockedInTime,ClockedOutTime,DayType")] Attendance attendance)
        {
            // ✅ Retrieve selected Employee and Event (including Service)
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == attendance.EmployeeId);
            var appEvent = await _context.AppEvent
                .Include(e => e.Service)
                .FirstOrDefaultAsync(e => e.Id == attendance.EventId);

            // ✅ Check for qualification mismatch
            if (employee == null || appEvent == null || employee.Qualifications != appEvent.Service.Qualifications)
            {
                ModelState.AddModelError("", "Selected employee does not meet the qualification requirement for this service.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(attendance);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ModelState.AddModelError("", "An error occurred while saving. Please try again.");
                }
            }

            // 🔁 Repopulate dropdowns if validation fails
            var events = await _context.AppEvent
                .Include(e => e.Service)
                .Select(e => new
                {
                    Id = e.Id,
                    DisplayText = $"{e.Service.Name}: ({e.StartTime:MMM-dd-yyyy HH:mm} - {e.EndTime:HH:mm})"
                })
                .ToListAsync();

            ViewData["EventId"] = new SelectList(events, "Id", "DisplayText", attendance.EventId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", attendance.EmployeeId);

            return View(attendance);
        }

        // GET: Attendance/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", attendance.EmployeeId);
            
            var events = _context.AppEvent
                .Include(e => e.Service)
                .Select(e => new
                {
                    Id = e.Id,
                    DisplayText = $"{(e.Service != null ? e.Service.Name : "Unknown Service")}: " +
                                  $"({e.StartTime:MMM-dd-yyyy HH:mm} - {e.EndTime:HH:mm})"
                })
                .ToList();
            
            ViewData["EventId"] = new SelectList(events, "Id", "DisplayText", attendance.EventId);
            return View(attendance);
        }

        // POST: Attendance/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AttendanceId,EmployeeId,EventId,ClockedInTime,ClockedOutTime,DayType")] Attendance attendance)
        {
            if (id != attendance.AttendanceId)
            {
                return NotFound();
            }

            // ✅ Retrieve selected Employee and Event (including Service)
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == attendance.EmployeeId);
            var appEvent = await _context.AppEvent
                .Include(e => e.Service)
                .FirstOrDefaultAsync(e => e.Id == attendance.EventId);
            
            if (employee == null || appEvent == null || employee.Qualifications != appEvent.Service.Qualifications)
            {
                ModelState.AddModelError("", "Selected employee does not meet the qualification requirement for this service.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attendance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendanceExists(attendance.AttendanceId))
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
            
            var events = await _context.AppEvent
                .Include(e => e.Service)
                .Select(e => new
                {
                    Id = e.Id,
                    DisplayText = $"{e.Service.Name}: ({e.StartTime:MMM-dd-yyyy HH:mm} - {e.EndTime:HH:mm})"
                })
                .ToListAsync();

            ViewData["EventId"] = new SelectList(events, "Id", "DisplayText", attendance.EventId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", attendance.EmployeeId);

            return View(attendance);
        }

        // GET: Attendance/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances
                .Include(a => a.Employee)
                .Include(a => a.Event)
                .FirstOrDefaultAsync(m => m.AttendanceId == id);
            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        // POST: Attendance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance != null)
            {
                _context.Attendances.Remove(attendance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AttendanceExists(int id)
        {
            return _context.Attendances.Any(e => e.AttendanceId == id);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClockIn(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);

            if (attendance == null)
            {
                return NotFound();
            }

            if (attendance.ClockedInTime == null)
            {
                attendance.ClockedInTime = DateTime.Now;
                _context.Update(attendance);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = attendance.AttendanceId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClockOut(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);

            if (attendance == null)
            {
                return NotFound();
            }

            if (attendance.ClockedInTime != null && attendance.ClockedOutTime == null)
            {
                attendance.ClockedOutTime = DateTime.Now; 
                _context.Update(attendance);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = attendance.AttendanceId });
        }
    }
}
