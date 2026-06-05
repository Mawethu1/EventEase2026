-- Run after `dotnet ef database update` (or the auto-migration in Program.cs).
-- Creates the consolidated view that the Bookings screen reads from.
IF OBJECT_ID('vw_BookingDetails', 'V') IS NOT NULL DROP VIEW vw_BookingDetails;
GO
CREATE VIEW vw_BookingDetails AS
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
    e.EventDate,
    e.Description
FROM Bookings b
INNER JOIN Venues v ON v.VenueId = b.VenueId
INNER JOIN Events e ON e.EventId = b.EventId;
GO
