using eventEaseBookingSystem.Data;
using eventEaseBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace eventEaseBookingSystem.Controllers
{
    public class VenuesController : Controller
    {
        private readonly MongoDbContext _context;

        public VenuesController(MongoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var venues = _context.Venues.Find(_ => true).ToList();
            return View(venues);
        }

        public IActionResult Details(int id)
        {
            var venue = _context.Venues.Find(v => v.VenueId == id).FirstOrDefault();
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Venue venue)
        {
            if (ModelState.IsValid)
            {
                _context.Venues.InsertOne(venue);
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        public IActionResult Edit(int id)
        {
            var venue = _context.Venues.Find(v => v.VenueId == id).FirstOrDefault();
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Venue venue)
        {
            if (id != venue.VenueId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _context.Venues.ReplaceOne(v => v.VenueId == id, venue);
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        public IActionResult Delete(int id)
        {
            var venue = _context.Venues.Find(v => v.VenueId == id).FirstOrDefault();
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var hasBookings = _context.Bookings.Find(b => b.VenueId == id).Any();
                if (hasBookings)
                {
                    ModelState.AddModelError("", "Cannot delete a venue with active bookings.");
                    return View();
                }

                _context.Venues.DeleteOne(v => v.VenueId == id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while deleting the venue: {ex.Message}");
                return View();
            }
        }
    }
}