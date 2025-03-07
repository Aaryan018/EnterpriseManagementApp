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
using Microsoft.Extensions.Logging; // For logging

namespace EnterpriseManagementApp.Controllers
{
    [Authorize(Roles = "Manager")]
    public class AssetsController : Controller
    {
        private readonly ManageHousingDbContext _context;
        private readonly ILogger<AssetsController> _logger; // Add logger

        public AssetsController(ManageHousingDbContext context, ILogger<AssetsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Assets
        public async Task<IActionResult> Index()
        {
            var assets = await _context.Assets.ToListAsync();
            _logger.LogInformation("Loaded {Count} assets", assets.Count);
            return View(assets);
        }

        // GET: Assets/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Details: ID is null or empty");
                return NotFound();
            }

            _logger.LogInformation("Details: Looking up asset with ID {Id}", id);
            var asset = await _context.Assets
                .FirstOrDefaultAsync(m => m.AssetId == id);
            if (asset == null)
            {
                _logger.LogWarning("Details: Asset not found with ID {Id}", id);
                return NotFound();
            }

            return View(asset);
        }

        // GET: Assets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Assets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RentRate,Name,Description,Type,Value,CustomerId,Status,Address")] Asset asset)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(asset.AssetId))
                    {
                        asset.AssetId = Guid.NewGuid().ToString();
                        _logger.LogInformation("Create: Generated new AssetId {AssetId}", asset.AssetId);
                    }

                    asset.CreatedAt = DateOnly.FromDateTime(DateTime.Today);
                    asset.UpdatedAt = DateOnly.FromDateTime(DateTime.Today);

                    _context.Add(asset);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Asset created successfully with ID {AssetId}", asset.AssetId);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving asset: {Message}", ex.Message);
                    ModelState.AddModelError("", "An error occurred while saving the asset. Please try again.");
                }
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogWarning("Validation error for {Field}: {ErrorMessage}", state.Key, error.ErrorMessage);
                    }
                }
            }
            return View(asset);
        }

        // GET: Assets/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Edit: ID is null or empty");
                return NotFound();
            }

            if (!Guid.TryParse(id, out var guidId))
            {
                _logger.LogWarning("Edit: Invalid GUID format for ID {Id}", id);
                return NotFound();
            }

            _logger.LogInformation("Edit: Looking up asset with ID {Id}", id);
            var asset = await _context.Assets
                .FirstOrDefaultAsync(m => m.AssetId == id);
            if (asset == null)
            {
                _logger.LogWarning("Edit: Asset not found with ID {Id}", id);
                return NotFound();
            }

            return View(asset);
        }

        // POST: Assets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("AssetId,RentRate,Name,Description,Type,Value,CustomerId,Status,Address,CreatedAt,UpdatedAt")] Asset asset)
        {
            if (id != asset.AssetId)
            {
                _logger.LogWarning("Edit: ID mismatch. URL ID {UrlId}, Asset ID {AssetId}", id, asset.AssetId);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asset);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!AssetExists(asset.AssetId))
                    {
                        _logger.LogWarning("Edit: Asset not found during update with ID {Id}", asset.AssetId);
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, "Edit: Concurrency error for Asset ID {Id}", asset.AssetId);
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(asset);
        }

        // GET: Assets/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Delete: ID is null or empty");
                return NotFound();
            }

            if (!Guid.TryParse(id, out var guidId))
            {
                _logger.LogWarning("Delete: Invalid GUID format for ID {Id}", id);
                return NotFound();
            }

            _logger.LogInformation("Delete: Looking up asset with ID {Id}", id);
            var asset = await _context.Assets
                .FirstOrDefaultAsync(m => m.AssetId == id);
            if (asset == null)
            {
                _logger.LogWarning("Delete: Asset not found with ID {Id}", id);
                return NotFound();
            }

            return View(asset);
        }

        // POST: Assets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var asset = await _context.Assets
                .FirstOrDefaultAsync(m => m.AssetId == id);
            if (asset != null)
            {
                _context.Assets.Remove(asset);
                await _context.SaveChangesAsync();
                _logger.LogInformation("DeleteConfirmed: Asset deleted with ID {Id}", id);
            }
            else
            {
                _logger.LogWarning("DeleteConfirmed: Asset not found with ID {Id}", id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AssetExists(string id)
        {
            return _context.Assets.Any(e => e.AssetId == id);
        }
    }
}