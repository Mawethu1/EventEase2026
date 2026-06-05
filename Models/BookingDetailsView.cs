namespace EventEase2026.Models;

/// <summary>
/// Keyless entity mapped to the SQL view vw_BookingDetails.
/// Consolidates Booking + Venue + Event data for the booking specialists' screen.
/// </summary>
public class BookingDetailsView
{
    public int BookingId { get; set; }
    public DateTime BookingDate { get; set; }

    public int VenueId { get; set; }
    public string VenueName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string? ImageUrl { get; set; }

    public int EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public int EventTypeId { get; set; }
    public string EventTypeName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string? Description { get; set; }
}
