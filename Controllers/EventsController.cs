using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase2026.Data;
using EventEase2026.Models;

namespace EventEase2026.Controllers;

public class EventsController : Controller
{
    private readonly ApplicationDbContext _db;
    public EventsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
        => View(await _db.Events.Include(e => e.EventType).AsNoTracking().ToListAsync());

    public async Task<IActionResult> Create()
    {
        await PopulateEventTypesAsync();
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Event ev)
    {
        if (!ModelState.IsValid)
        {
            await PopulateEventTypesAsync();
            return View(ev);
        }

        try
        {
            _db.Events.Add(ev);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Event created.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Could not create event: {ex.Message}";
            await PopulateEventTypesAsync();
            return View(ev);
        }
    }

    private async Task PopulateEventTypesAsync(int? selectedEventTypeId = null)
    {
        ViewBag.EventTypes = new SelectList(
            await _db.EventTypes.AsNoTracking().OrderBy(et => et.Name).ToListAsync(),
            "EventTypeId", "Name", selectedEventTypeId);
    }
    public async Task<IActionResult> Edit(int id)
    {
        var ev = await _db.Events.FindAsync(id);
        if (ev == null) return NotFound();
        await PopulateEventTypesAsync(ev.EventTypeId);
        return View(ev);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Event input)
    {
        if (id != input.EventId) return BadRequest();
        if (!ModelState.IsValid)
        {
            await PopulateEventTypesAsync(input.EventTypeId);
            return View(input);
        }
        try
        {
            _db.Update(input);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Event updated.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Could not update event: {ex.Message}";
            await PopulateEventTypesAsync(input.EventTypeId);
            return View(input);
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        var ev = await _db.Events.AsNoTracking().FirstOrDefaultAsync(e => e.EventId == id);
        if (ev == null) return NotFound();
        return View(ev);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ev = await _db.Events.Include(e => e.Bookings).FirstOrDefaultAsync(e => e.EventId == id);
        if (ev == null) return NotFound();

        if (ev.Bookings.Any())
        {
            TempData["Error"] = $"Cannot delete '{ev.EventName}' because it has {ev.Bookings.Count} active booking(s).";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _db.Events.Remove(ev);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Event deleted.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Delete failed: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}
