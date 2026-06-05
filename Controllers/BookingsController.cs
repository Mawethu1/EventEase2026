using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase2026.Data;
using EventEase2026.Models;

namespace EventEase2026.Controllers;

public class BookingsController : Controller
{
    private readonly ApplicationDbContext _db;
    public BookingsController(ApplicationDbContext db) => _db = db;

    /// <summary>
    /// Consolidated view (vw_BookingDetails) with search and filters.
    /// </summary>
    public async Task<IActionResult> Index(
        string? searchTerm,
        int? eventTypeId,
        int? venueId,
        DateTime? eventStart,
        DateTime? eventEnd,
        bool availableOnly = false)
    {
        ViewData["SearchTerm"] = searchTerm;
        ViewData["EventTypeId"] = eventTypeId;
        ViewData["VenueId"] = venueId;
        ViewData["EventStart"] = eventStart?.ToString("yyyy-MM-dd");
        ViewData["EventEnd"] = eventEnd?.ToString("yyyy-MM-dd");
        ViewData["AvailableOnly"] = availableOnly;

        await PopulateFiltersAsync();

        var query = _db.BookingDetails.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            var like = $"%{EscapeLike(term)}%";

            if (int.TryParse(term, out var id))
            {
                query = query.Where(b =>
                    b.BookingId == id ||
                    EF.Functions.Like(b.EventName, like, @"\") ||
                    EF.Functions.Like(b.VenueName, like, @"\") ||
                    EF.Functions.Like(b.Location, like, @"\"));
            }
            else
            {
                query = query.Where(b =>
                    EF.Functions.Like(b.EventName, like, @"\") ||
                    EF.Functions.Like(b.VenueName, like, @"\") ||
                    EF.Functions.Like(b.Location, like, @"\"));
            }
        }

        if (eventTypeId.HasValue)
        {
            query = query.Where(b => b.EventTypeId == eventTypeId.Value);
        }

        if (venueId.HasValue)
        {
            query = query.Where(b => b.VenueId == venueId.Value);
        }

        if (eventStart.HasValue)
        {
            query = query.Where(b => b.EventDate >= eventStart.Value.Date);
        }

        if (eventEnd.HasValue)
        {
            query = query.Where(b => b.EventDate <= eventEnd.Value.Date.AddDays(1).AddTicks(-1));
        }

        if (availableOnly)
        {
            var rangeStart = eventStart?.Date ?? DateTime.MinValue;
            var rangeEnd = eventEnd?.Date.AddDays(1).AddTicks(-1) ?? DateTime.MaxValue;

            query = query.Where(b => !_db.Bookings.Any(o =>
                o.VenueId == b.VenueId &&
                o.BookingId != b.BookingId &&
                o.BookingDate >= rangeStart &&
                o.BookingDate <= rangeEnd));
        }

        var data = await query.OrderByDescending(b => b.BookingDate).ToListAsync();
        return View(data);
    }

    private static string EscapeLike(string value)
        => value.Replace(@"\", @"\\").Replace("%", @"\%").Replace("_", @"\_");

    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new Booking { BookingDate = DateTime.Today.AddDays(1) });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Booking booking)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(booking);
            return View(booking);
        }

        try
        {
            // Double-booking validation: same venue cannot be booked twice on the same date/time.
            var clash = await _db.Bookings.AnyAsync(b =>
                b.VenueId == booking.VenueId && b.BookingDate == booking.BookingDate);

            if (clash)
            {
                ModelState.AddModelError(string.Empty,
                    "This venue is already booked for the selected date/time. Please pick another slot.");
                TempData["Error"] = "Double booking prevented.";
                await PopulateDropdownsAsync(booking);
                return View(booking);
            }

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Booking #{booking.BookingId} created.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "Could not save the booking. The slot may already be taken.");
            await PopulateDropdownsAsync(booking);
            return View(booking);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Unexpected error: {ex.Message}";
            await PopulateDropdownsAsync(booking);
            return View(booking);
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        var booking = await _db.Bookings
            .Include(b => b.Venue)
            .Include(b => b.Event)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BookingId == id);
        if (booking == null) return NotFound();
        return View(booking);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking == null) return NotFound();
        try
        {
            _db.Bookings.Remove(booking);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Booking cancelled.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Delete failed: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync(Booking? selected = null)
    {
        ViewBag.Venues = new SelectList(
            await _db.Venues.AsNoTracking().OrderBy(v => v.VenueName).ToListAsync(),
            "VenueId", "VenueName", selected?.VenueId);

        ViewBag.Events = new SelectList(
            await _db.Events.AsNoTracking().OrderBy(e => e.EventName).ToListAsync(),
            "EventId", "EventName", selected?.EventId);
    }

    private async Task PopulateFiltersAsync()
    {
        ViewBag.EventTypes = new SelectList(
            await _db.EventTypes.AsNoTracking().OrderBy(et => et.Name).ToListAsync(),
            "EventTypeId", "Name");

        ViewBag.Venues = new SelectList(
            await _db.Venues.AsNoTracking().OrderBy(v => v.VenueName).ToListAsync(),
            "VenueId", "VenueName");
    }
}
