using System.ComponentModel.DataAnnotations;

namespace eventEaseBookingSystem.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        
        [Required]
        public int EventId { get; set; }
        
        [Required]
        public int VenueId { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }
        
        [Required]
        [StringLength(100)]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Customer Email")]
        public string CustomerEmail { get; set; } = string.Empty;
          [Phone]
        [StringLength(15)]
        [Display(Name = "Customer Phone")]
        public string? CustomerPhone { get; set; }
        
        // Navigation properties
        public virtual Event? Event { get; set; }
        public virtual Venue? Venue { get; set; }
    }
}
