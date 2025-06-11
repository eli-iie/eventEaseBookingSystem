using System.ComponentModel.DataAnnotations;

namespace eventEaseBookingSystem.Models
{
    public class Venue
    {
        public int VenueId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string VenueName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;
        
        [Required]
        [Range(1, 10000)]
        public int Capacity { get; set; }
          public string? ImageUrl { get; set; }
        
        [Required]
        public bool IsAvailable { get; set; } = true;
        
        // Navigation property
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
