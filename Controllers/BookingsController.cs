using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eventEaseBookingSystem.Data;
using eventEaseBookingSystem.Models;

namespace eventEaseBookingSystem.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }        // GET: Bookings
        public async Task<IActionResult> Index(string searchString, string eventType, DateTime? startDate, DateTime? endDate, string venueAvailability)
        {
            var bookingsQuery = _context.Bookings.Include(b => b.Event).Include(b => b.Venue).AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                bookingsQuery = FilterBySearchString(bookingsQuery, searchString);
            }

            if (!string.IsNullOrEmpty(eventType))
            {
                bookingsQuery = FilterByEventType(bookingsQuery, eventType);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                bookingsQuery = FilterByDateRange(bookingsQuery, startDate, endDate);
            }

            // Apply venue availability filter
            if (!string.IsNullOrEmpty(venueAvailability))
            {
                bookingsQuery = FilterByVenueAvailability(bookingsQuery, venueAvailability);
            }

            // Get distinct event types for dropdown
            ViewBag.EventTypes = await _context.Events
                .Select(e => e.EventType)
                .Distinct()
                .OrderBy(et => et)
                .ToListAsync();

            return View(await bookingsQuery.ToListAsync());
        }

        private static IQueryable<Booking> FilterByEventType(IQueryable<Booking> query, string eventType)
        {
            return query.Where(b => b.Event != null && b.Event.EventType.ToString() == eventType);
        }        private static IQueryable<Booking> FilterByDateRange(IQueryable<Booking> query, DateTime? startDate, DateTime? endDate)
        {
            return query.Where(b => b.BookingDate >= startDate && b.BookingDate <= endDate);
        }        private static IQueryable<Booking> FilterBySearchString(IQueryable<Booking> query, string searchString)
        {
            return query.Where(b => 
                (b.CustomerName != null && b.CustomerName.Contains(searchString)) ||
                (b.CustomerEmail != null && b.CustomerEmail.Contains(searchString)) ||
                (b.Event != null && b.Event.EventName != null && b.Event.EventName.Contains(searchString)) ||
                (b.Event != null && b.Event.Description != null && b.Event.Description.Contains(searchString)) ||
                (b.Venue != null && b.Venue.VenueName != null && b.Venue.VenueName.Contains(searchString)) ||
                (b.Venue != null && b.Venue.Location != null && b.Venue.Location.Contains(searchString)) ||
                b.BookingId.ToString().Contains(searchString)
            );
        }

        private static IQueryable<Booking> FilterByVenueAvailability(IQueryable<Booking> query, string venueAvailability)
        {
            if (venueAvailability == "available")
            {
                return query.Where(b => b.Venue != null && b.Venue.IsAvailable);
            }
            else if (venueAvailability == "unavailable")
            {
                return query.Where(b => b.Venue != null && !b.Venue.IsAvailable);
            }
            return query;
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!ModelState.IsValid || id == null)
            {
                return BadRequest();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        // Define constants for repeated literals
        private const string EventIdLiteral = "EventId";
        private const string EventNameLiteral = "EventName";
        private const string VenueIdLiteral = "VenueId";
        private const string VenueNameLiteral = "VenueName";        public IActionResult Create()
        {
            ViewData[EventIdLiteral] = new SelectList(_context.Events, EventIdLiteral, EventNameLiteral);
            
            // Create venue list with availability status
            var venues = _context.Venues.Select(v => new {
                VenueId = v.VenueId,
                DisplayText = v.VenueName + (v.IsAvailable ? " (Available)" : " (Unavailable)")
            }).ToList();
            ViewData[VenueIdLiteral] = new SelectList(venues, "VenueId", "DisplayText");
            
            return View();
        }// POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,VenueId,BookingDate,CustomerName,CustomerEmail,CustomerPhone")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                // Check for double booking
                var existingBooking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.VenueId == booking.VenueId && b.BookingDate.Date == booking.BookingDate.Date);
                  if (existingBooking != null)
                {
                    ModelState.AddModelError("", "This venue is already booked for the selected date.");
                    ViewData[EventIdLiteral] = new SelectList(_context.Events, EventIdLiteral, EventNameLiteral, booking.EventId);
                    
                    // Create venue list with availability status for error case
                    var venues = _context.Venues.Select(v => new {
                        VenueId = v.VenueId,
                        DisplayText = v.VenueName + (v.IsAvailable ? " (Available)" : " (Unavailable)")
                    }).ToList();
                    ViewData[VenueIdLiteral] = new SelectList(venues, "VenueId", "DisplayText", booking.VenueId);
                    
                    return View(booking);
                }

                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData[EventIdLiteral] = new SelectList(_context.Events, EventIdLiteral, EventNameLiteral, booking.EventId);
            
            // Create venue list with availability status for validation error case
            var venuesForError = _context.Venues.Select(v => new {
                VenueId = v.VenueId,
                DisplayText = v.VenueName + (v.IsAvailable ? " (Available)" : " (Unavailable)")
            }).ToList();
            ViewData[VenueIdLiteral] = new SelectList(venuesForError, "VenueId", "DisplayText", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!ModelState.IsValid || id == null)
            {
                return BadRequest();
            }            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData[EventIdLiteral] = new SelectList(_context.Events, EventIdLiteral, EventNameLiteral, booking.EventId);
            
            // Create venue list with availability status for edit
            var venues = _context.Venues.Select(v => new {
                VenueId = v.VenueId,
                DisplayText = v.VenueName + (v.IsAvailable ? " (Available)" : " (Unavailable)")
            }).ToList();
            ViewData[VenueIdLiteral] = new SelectList(venues, "VenueId", "DisplayText", booking.VenueId);
            
            return View(booking);
        }        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,EventId,VenueId,BookingDate,CustomerName,CustomerEmail,CustomerPhone")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check for double booking (excluding current booking)
                    var existingBooking = await _context.Bookings
                        .FirstOrDefaultAsync(b => b.VenueId == booking.VenueId && 
                                                 b.BookingDate.Date == booking.BookingDate.Date && 
                                                 b.BookingId != booking.BookingId);
                      if (existingBooking != null)
                    {
                        ModelState.AddModelError("", "This venue is already booked for the selected date.");
                        ViewData[EventIdLiteral] = new SelectList(_context.Events, EventIdLiteral, EventNameLiteral, booking.EventId);
                        
                        // Create venue list with availability status for error case
                        var venues = _context.Venues.Select(v => new {
                            VenueId = v.VenueId,
                            DisplayText = v.VenueName + (v.IsAvailable ? " (Available)" : " (Unavailable)")
                        }).ToList();
                        ViewData[VenueIdLiteral] = new SelectList(venues, "VenueId", "DisplayText", booking.VenueId);
                        
                        return View(booking);
                    }

                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }                return RedirectToAction(nameof(Index));
            }
            ViewData[EventIdLiteral] = new SelectList(_context.Events, EventIdLiteral, EventNameLiteral, booking.EventId);
            
            // Create venue list with availability status for validation error case
            var venuesForValidationError = _context.Venues.Select(v => new {
                VenueId = v.VenueId,
                DisplayText = v.VenueName + (v.IsAvailable ? " (Available)" : " (Unavailable)")
            }).ToList();
            ViewData[VenueIdLiteral] = new SelectList(venuesForValidationError, "VenueId", "DisplayText", booking.VenueId);
            
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!ModelState.IsValid || id == null)
            {
                return BadRequest();
            }

            return await Details(id);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
