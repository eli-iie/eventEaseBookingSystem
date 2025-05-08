using eventEaseBookingSystem.Data;
using eventEaseBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;

namespace eventEaseBookingSystem.Controllers
{
    public class EventsController : Controller
    {
        private readonly MongoDbContext _context;

        public EventsController(MongoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var events = _context.Events.Find(_ => true).ToList();
            return View(events);
        }

        public IActionResult Details(int id)
        {
            var eventItem = _context.Events.Find(e => e.EventId == id).FirstOrDefault();
            if (eventItem == null)
            {
                return NotFound();
            }
            return View(eventItem);
        }

        public IActionResult Create()
        {
            var venues = _context.Venues.Find(_ => true).ToList();
            ViewBag.Venues = venues;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Event eventItem)
        {
            if (string.IsNullOrEmpty(eventItem.VenueId) || !ObjectId.TryParse(eventItem.VenueId, out _))
            {
                ModelState.AddModelError("VenueId", "Invalid VenueId. Please select a valid venue.");
            }

            if (ModelState.IsValid)
            {
                _context.Events.InsertOne(eventItem);
                return RedirectToAction(nameof(Index));
            }

            var venues = _context.Venues.Find(_ => true).ToList();
            ViewBag.Venues = venues;
            return View(eventItem);
        }

        public IActionResult Edit(int id)
        {
            var eventItem = _context.Events.Find(e => e.EventId == id).FirstOrDefault();
            if (eventItem == null)
            {
                return NotFound();
            }
            return View(eventItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Event eventItem)
        {
            if (id != eventItem.EventId)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(eventItem.VenueId) || !ObjectId.TryParse(eventItem.VenueId, out _))
            {
                ModelState.AddModelError("VenueId", "Please select a valid venue.");
                var venues = _context.Venues.Find(_ => true).ToList();
                ViewBag.Venues = venues;
                return View(eventItem);
            }

            if (ModelState.IsValid)
            {
                _context.Events.ReplaceOne(e => e.EventId == id, eventItem);
                return RedirectToAction(nameof(Index));
            }
            return View(eventItem);
        }

        public IActionResult Delete(int id)
        {
            var eventItem = _context.Events.Find(e => e.EventId == id).FirstOrDefault();
            if (eventItem == null)
            {
                return NotFound();
            }
            return View(eventItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var hasBookings = _context.Bookings.Find(b => b.EventId == id).Any();
                if (hasBookings)
                {
                    ModelState.AddModelError("", "Cannot delete an event with active bookings.");
                    return View();
                }

                _context.Events.DeleteOne(e => e.EventId == id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while deleting the event: {ex.Message}");
                return View();
            }
        }
    }
}