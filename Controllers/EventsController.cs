using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eventEaseBookingSystem.Data;
using eventEaseBookingSystem.Models;

namespace eventEaseBookingSystem.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }        // GET: Events
        public async Task<IActionResult> Index(string searchString, EventType? eventType, 
            DateTime? startDate, DateTime? endDate, int? venueId)
        {
            var events = from e in _context.Events.Include(e => e.Venue)
                         select e;            if (!String.IsNullOrEmpty(searchString))
            {
                events = events.Where(e => e.EventName.Contains(searchString) 
                                        || (e.Description != null && e.Description.Contains(searchString))
                                        || e.Venue.VenueName.Contains(searchString));
            }

            if (eventType.HasValue)
            {
                events = events.Where(e => e.EventType == eventType.Value);
            }

            if (startDate.HasValue)
            {
                events = events.Where(e => e.EventDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                events = events.Where(e => e.EventDate <= endDate.Value);
            }

            if (venueId.HasValue)
            {
                events = events.Where(e => e.VenueId == venueId.Value);
            }

            // Prepare filter dropdown data
            ViewData["CurrentFilter"] = searchString;
            ViewData["EventType"] = eventType;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
            ViewData["VenueId"] = venueId;
            ViewData["Venues"] = new SelectList(_context.Venues, "VenueId", "VenueName", venueId);
            ViewData["EventTypes"] = new SelectList(Enum.GetValues(typeof(EventType)).Cast<EventType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() }), "Value", "Text", (int?)eventType);

            return View(await events.OrderBy(e => e.EventDate).ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");
            return View();
        }        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventName,EventDate,Description,EventType,VenueId")] Event eventItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eventItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", eventItem.VenueId);
            return View(eventItem);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", eventItem.VenueId);
            return View(eventItem);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,EventDate,Description,EventType,VenueId")] Event eventItem)
        {
            if (id != eventItem.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(eventItem.EventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", eventItem.VenueId);
            return View(eventItem);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem != null)
            {
                // Check if event has active bookings
                var hasBookings = await _context.Bookings.AnyAsync(b => b.EventId == id);
                if (hasBookings)
                {
                    ModelState.AddModelError("", "Cannot delete event with active bookings.");
                    var eventWithVenue = await _context.Events
                        .Include(e => e.Venue)
                        .FirstOrDefaultAsync(m => m.EventId == id);
                    return View(eventWithVenue);
                }

                _context.Events.Remove(eventItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
