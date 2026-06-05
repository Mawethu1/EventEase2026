using System.ComponentModel.DataAnnotations;

namespace EventEase2026.Models;

public class Venue
{
    public int VenueId { get; set; }

    [Required, StringLength(150)]
    public string VenueName { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string Location { get; set; } = string.Empty;

    [Range(1, 100000)]
    public int Capacity { get; set; }

    /// <summary>URL of the image stored in the Azurite venue-images container.</summary>
    public string? ImageUrl { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
