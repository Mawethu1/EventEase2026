using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase2026.Data;
using EventEase2026.Models;
using EventEase2026.Services;

namespace EventEase2026.Controllers;

public class VenuesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IBlobStorageService _blob;

    public VenuesController(ApplicationDbContext db, IBlobStorageService blob)
    {
        _db = db;
        _blob = blob;
    }

    public async Task<IActionResult> Index()
        => View(await _db.Venues.AsNoTracking().ToListAsync());

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Venue venue, IFormFile? imageFile)
    {
        if (!ModelState.IsValid) return View(venue);

        try
        {
            if (imageFile is { Length: > 0 })
            {
                if (!IsValidImage(imageFile))
                {
                    ModelState.AddModelError("imageFile", "Only JPG, PNG or WEBP images up to 5 MB are allowed.");
                    return View(venue);
                }

                using var stream = imageFile.OpenReadStream();
                venue.ImageUrl = await _blob.UploadAsync(stream, imageFile.FileName, imageFile.ContentType);
            }

            _db.Venues.Add(venue);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Venue '{venue.VenueName}' created.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Could not create venue: {ex.Message}";
            return View(venue);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var venue = await _db.Venues.FindAsync(id);
        if (venue == null) return NotFound();
        return View(venue);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Venue input, IFormFile? imageFile)
    {
        if (id != input.VenueId) return BadRequest();
        if (!ModelState.IsValid) return View(input);

        var venue = await _db.Venues.FindAsync(id);
        if (venue == null) return NotFound();

        try
        {
            venue.VenueName = input.VenueName;
            venue.Location = input.Location;
            venue.Capacity = input.Capacity;

            if (imageFile is { Length: > 0 })
            {
                if (!IsValidImage(imageFile))
                {
                    ModelState.AddModelError("imageFile", "Only JPG, PNG or WEBP images up to 5 MB are allowed.");
                    return View(input);
                }

                if (!string.IsNullOrEmpty(venue.ImageUrl))
                    await _blob.DeleteAsync(venue.ImageUrl);

                using var stream = imageFile.OpenReadStream();
                venue.ImageUrl = await _blob.UploadAsync(stream, imageFile.FileName, imageFile.ContentType);
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "Venue updated.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Could not update venue: {ex.Message}";
            return View(input);
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        var venue = await _db.Venues.AsNoTracking().FirstOrDefaultAsync(v => v.VenueId == id);
        if (venue == null) return NotFound();
        return View(venue);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var venue = await _db.Venues.Include(v => v.Bookings).FirstOrDefaultAsync(v => v.VenueId == id);
        if (venue == null) return NotFound();

        // Validation: do NOT allow deleting a venue that has active bookings.
        if (venue.Bookings.Any())
        {
            TempData["Error"] = $"Cannot delete '{venue.VenueName}' because it has {venue.Bookings.Count} active booking(s).";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            if (!string.IsNullOrEmpty(venue.ImageUrl))
                await _blob.DeleteAsync(venue.ImageUrl);

            _db.Venues.Remove(venue);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Venue deleted.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Delete failed: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }

    private static bool IsValidImage(IFormFile file)
    {
        if (file.Length > 5 * 1024 * 1024) return false;
        var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
        return allowed.Contains(file.ContentType.ToLowerInvariant());
    }
}
