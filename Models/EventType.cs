using System.ComponentModel.DataAnnotations;

namespace EventEase2026.Models;

public class EventType
{
    public int EventTypeId { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Event> Events { get; set; } = new List<Event>();
}
