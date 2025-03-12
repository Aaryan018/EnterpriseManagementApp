//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using EnterpriseManagementApp.Data;
////using EnterpriseManagementApp.Models.Rentals;

//namespace EnterpriseManagementApp.Controllers
//{
//    public class LeaseAgreementsController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public LeaseAgreementsController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // GET: LeaseAgreements
//        public async Task<IActionResult> Index()
//        {
//            return View(await _context.LeaseAgreements.ToListAsync());
//        }

//        // GET: LeaseAgreements/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var leaseAgreement = await _context.LeaseAgreements
//                .FirstOrDefaultAsync(m => m.LeaseID == id);
//            if (leaseAgreement == null)
//            {
//                return NotFound();
//            }

//            return View(leaseAgreement);
//        }

//        // GET: LeaseAgreements/Create
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // POST: LeaseAgreements/Create
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("LeaseID,RenterID,AssetID,StartDate,EndDate,RentAmount")] LeaseAgreement leaseAgreement)
//        {
//            if (ModelState.IsValid)
//            {
//                _context.Add(leaseAgreement);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            return View(leaseAgreement);
//        }

//        // GET: LeaseAgreements/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var leaseAgreement = await _context.LeaseAgreements.FindAsync(id);
//            if (leaseAgreement == null)
//            {
//                return NotFound();
//            }
//            return View(leaseAgreement);
//        }

//        // POST: LeaseAgreements/Edit/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("LeaseID,RenterID,AssetID,StartDate,EndDate,RentAmount")] LeaseAgreement leaseAgreement)
//        {
//            if (id != leaseAgreement.LeaseID)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(leaseAgreement);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!LeaseAgreementExists(leaseAgreement.LeaseID))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            return View(leaseAgreement);
//        }

//        // GET: LeaseAgreements/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var leaseAgreement = await _context.LeaseAgreements
//                .FirstOrDefaultAsync(m => m.LeaseID == id);
//            if (leaseAgreement == null)
//            {
//                return NotFound();
//            }

//            return View(leaseAgreement);
//        }

//        // POST: LeaseAgreements/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var leaseAgreement = await _context.LeaseAgreements.FindAsync(id);
//            if (leaseAgreement != null)
//            {
//                _context.LeaseAgreements.Remove(leaseAgreement);
//            }

//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool LeaseAgreementExists(int id)
//        {
//            return _context.LeaseAgreements.Any(e => e.LeaseID == id);
//        }
//    }
//}
