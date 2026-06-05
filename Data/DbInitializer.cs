using Microsoft.EntityFrameworkCore;
using EventEase2026.Models;

namespace EventEase2026.Data;

public static class DbInitializer
{
    public static async Task EnsureBookingDetailsViewAsync(ApplicationDbContext db)
    {
        // Recreate the booking details view used by BookingDetailsView.
        var sql = """
                  IF OBJECT_ID(N'dbo.vw_BookingDetails', N'V') IS NOT NULL
                      DROP VIEW dbo.vw_BookingDetails;
                  EXEC(N'
                      CREATE VIEW dbo.vw_BookingDetails AS
                      SELECT
                          b.BookingId,
                          b.BookingDate,
                          v.VenueId,
                          v.VenueName,
                          v.Location,
                          v.Capacity,
                          v.ImageUrl,
                          e.EventId,
                          e.EventName,
                          e.EventTypeId,
                          et.Name AS EventTypeName,
                          e.EventDate,
                          e.Description
                      FROM dbo.Bookings b
                      INNER JOIN dbo.Venues v ON v.VenueId = b.VenueId
                      INNER JOIN dbo.Events e ON e.EventId = b.EventId
                      INNER JOIN dbo.EventTypes et ON et.EventTypeId = e.EventTypeId
                  ')
                  """;

        await db.Database.ExecuteSqlRawAsync(sql);
    }

    public static async Task SeedAsync(ApplicationDbContext db)
    {
        // Seed just enough data for the app to be usable.
        if (!await db.Venues.AnyAsync())
        {
            db.Venues.AddRange(
                new Venue { VenueName = "City Hall", Location = "Downtown", Capacity = 500 },
                new Venue { VenueName = "Riverside Pavilion", Location = "Riverside", Capacity = 250 }
            );
        }

        var requiredTypes = new[]
        {
            "Unspecified",
            "Wedding",
            "Conference",
            "Meetup",
            "Concert",
            "Workshop",
            "Private Party",
            "Corporate Event"
        };

        var existingTypeNames = await db.EventTypes.Select(et => et.Name).ToListAsync();
        var missingTypes = requiredTypes
            .Except(existingTypeNames, StringComparer.OrdinalIgnoreCase)
            .Select(name => new EventType { Name = name })
            .ToList();

        if (missingTypes.Any())
        {
            db.EventTypes.AddRange(missingTypes);
            await db.SaveChangesAsync();
        }

        if (!await db.Events.AnyAsync())
        {
            var wedding = await db.EventTypes.FirstAsync(et => et.Name == "Wedding");
            var meetup = await db.EventTypes.FirstAsync(et => et.Name == "Meetup");
            var conference = await db.EventTypes.FirstAsync(et => et.Name == "Conference");
            var concert = await db.EventTypes.FirstAsync(et => et.Name == "Concert");

            db.Events.AddRange(
                new Event { EventName = "Wedding Reception", EventTypeId = wedding.EventTypeId, EventDate = DateTime.Today.AddDays(14).AddHours(15), Description = "Reception booking" },
                new Event { EventName = "Tech Meetup", EventTypeId = meetup.EventTypeId, EventDate = DateTime.Today.AddDays(7).AddHours(18), Description = "Community meetup" },
                new Event { EventName = "Startup Conference", EventTypeId = conference.EventTypeId, EventDate = DateTime.Today.AddDays(21).AddHours(9), Description = "Business conference" },
                new Event { EventName = "Summer Concert", EventTypeId = concert.EventTypeId, EventDate = DateTime.Today.AddDays(30).AddHours(19), Description = "Live music evening" }
            );
        }

        await db.SaveChangesAsync();
    }
}

