using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eventEaseBookingSystem.Data;
using eventEaseBookingSystem.Models;

namespace eventEaseBookingSystem.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reports
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            // Generate summary statistics
            ViewBag.TotalBookings = bookings.Count;
            ViewBag.TotalVenues = await _context.Venues.CountAsync();
            ViewBag.TotalEvents = await _context.Events.CountAsync();
            ViewBag.BookingsToday = bookings.Count(b => b.BookingDate.Date == DateTime.Today);
            
            // Event type statistics
            var eventTypeStats = await _context.Events
                .GroupBy(e => e.EventType)
                .Select(g => new { EventType = g.Key, Count = g.Count() })
                .ToListAsync();
            ViewBag.EventTypeStats = eventTypeStats;

            // Monthly booking trends
            var monthlyBookings = bookings
                .Where(b => b.BookingDate >= DateTime.Now.AddMonths(-12))
                .GroupBy(b => new { Year = b.BookingDate.Year, Month = b.BookingDate.Month })
                .Select(g => new { 
                    Period = $"{g.Key.Year}-{g.Key.Month:D2}", 
                    Count = g.Count() 
                })
                .OrderBy(x => x.Period)
                .ToList();
            ViewBag.MonthlyBookings = monthlyBookings;

            // Popular venues
            var popularVenues = await _context.Bookings
                .Include(b => b.Venue)
                .GroupBy(b => b.Venue)
                .Select(g => new { 
                    Venue = g.Key, 
                    BookingCount = g.Count() 
                })
                .OrderByDescending(x => x.BookingCount)
                .Take(5)
                .ToListAsync();
            ViewBag.PopularVenues = popularVenues;

            return View(bookings);
        }

        // GET: Reports/ConsolidatedBookings
        public async Task<IActionResult> ConsolidatedBookings(string searchString, EventType? eventType, 
            DateTime? startDate, DateTime? endDate, int? venueId)
        {
            var bookings = from b in _context.Bookings
                          .Include(b => b.Event)
                          .Include(b => b.Venue)
                          select b;

            if (!String.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b => b.Event.EventName.Contains(searchString) 
                                            || b.Venue.VenueName.Contains(searchString));
            }

            if (eventType.HasValue)
            {
                bookings = bookings.Where(b => b.Event.EventType == eventType.Value);
            }

            if (startDate.HasValue)
            {
                bookings = bookings.Where(b => b.BookingDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                bookings = bookings.Where(b => b.BookingDate <= endDate.Value);
            }

            if (venueId.HasValue)
            {
                bookings = bookings.Where(b => b.VenueId == venueId.Value);
            }

            // Set filter values for the view
            ViewData["CurrentFilter"] = searchString;
            ViewData["EventType"] = eventType;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
            ViewData["VenueId"] = venueId;
            ViewData["Venues"] = await _context.Venues.ToListAsync();
            ViewData["EventTypes"] = Enum.GetValues(typeof(EventType)).Cast<EventType>().ToList();

            var result = await bookings.OrderByDescending(b => b.BookingDate).ToListAsync();
            
            return View(result);
        }

        // GET: Reports/VenueUtilization
        public async Task<IActionResult> VenueUtilization()
        {
            var venues = await _context.Venues
                .Select(v => new
                {
                    Venue = v,
                    TotalBookings = _context.Bookings.Count(b => b.VenueId == v.VenueId),
                    UniqueEvents = _context.Events.Count(e => e.VenueId == v.VenueId),
                    LastBooking = _context.Bookings
                        .Where(b => b.VenueId == v.VenueId)
                        .Max(b => (DateTime?)b.BookingDate),
                    UtilizationRate = _context.Bookings.Count(b => b.VenueId == v.VenueId) > 0 
                        ? (double)_context.Bookings.Count(b => b.VenueId == v.VenueId) / v.Capacity * 100 
                        : 0
                })
                .ToListAsync();

            return View(venues);
        }
    }
}
