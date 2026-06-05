using System.ComponentModel.DataAnnotations;

namespace EventEase2026.Models;

public class Event
{
    public int EventId { get; set; }

    [Required, StringLength(200)]
    public string EventName { get; set; } = string.Empty;

    [DataType(DataType.DateTime)]
    public DateTime EventDate { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public int EventTypeId { get; set; }
    public EventType? EventType { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
