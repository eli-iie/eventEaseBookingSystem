using System.ComponentModel.DataAnnotations;

namespace eventEaseBookingSystem.Models
{
    public enum EventType
    {
        Conference,
        Wedding,
        Concert,
        Workshop,
        Exhibition,
        Party,
        Meeting,
        Sport,
        Festival,
        Other
    }

    public class Event
    {
        public int EventId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string EventName { get; set; } = string.Empty;
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        public EventType EventType { get; set; } = EventType.Other;
          [Required]
        public int VenueId { get; set; }
        
        // Navigation properties
        public virtual Venue? Venue { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
