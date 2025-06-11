using Microsoft.EntityFrameworkCore;
using eventEaseBookingSystem.Models;

namespace eventEaseBookingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);            // Configure relationships
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Venue)
                .WithMany()
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Event)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Venue)
                .WithMany(v => v.Bookings)
                .HasForeignKey(b => b.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed sample data
            modelBuilder.Entity<Venue>().HasData(
                new Venue 
                { 
                    VenueId = 1, 
                    VenueName = "Grand Ballroom", 
                    Location = "Downtown Convention Center", 
                    Capacity = 500,
                    ImageUrl = "https://images.unsplash.com/photo-1519167758481-83f29b1fe317?w=400"
                },
                new Venue 
                { 
                    VenueId = 2, 
                    VenueName = "Garden Pavilion", 
                    Location = "City Park", 
                    Capacity = 200,
                    ImageUrl = "https://images.unsplash.com/photo-1464366400600-7168b8af9bc3?w=400"
                },
                new Venue 
                { 
                    VenueId = 3, 
                    VenueName = "Conference Hall A", 
                    Location = "Business District", 
                    Capacity = 100,
                    ImageUrl = "https://images.unsplash.com/photo-1475721027785-f74eccf877e2?w=400"
                }
            );

            modelBuilder.Entity<Event>().HasData(
                new Event 
                { 
                    EventId = 1, 
                    EventName = "Annual Gala", 
                    EventDate = DateTime.Today.AddDays(30), 
                    Description = "Annual company gala event",
                    VenueId = 1
                },
                new Event 
                { 
                    EventId = 2, 
                    EventName = "Product Launch", 
                    EventDate = DateTime.Today.AddDays(15), 
                    Description = "New product launch presentation",
                    VenueId = 3
                }
            );

            modelBuilder.Entity<Booking>().HasData(
                new Booking 
                { 
                    BookingId = 1, 
                    EventId = 1, 
                    VenueId = 1, 
                    BookingDate = DateTime.Today.AddDays(30)
                },
                new Booking 
                { 
                    BookingId = 2, 
                    EventId = 2, 
                    VenueId = 3, 
                    BookingDate = DateTime.Today.AddDays(15)
                }
            );
        }
    }
}
