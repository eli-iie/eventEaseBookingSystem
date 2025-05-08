using eventEaseBookingSystem.Data;
using eventEaseBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace eventEaseBookingSystem.Controllers
{
    public class BookingsController : Controller
    {
        private readonly MongoDbContext _context;

        public BookingsController(MongoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var bookings = _context.Bookings.Find(_ => true).ToList();
            return View(bookings);
        }

        public IActionResult Details(int id)
        {
            var booking = _context.Bookings.Find(b => b.BookingId == id).FirstOrDefault();
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                var isDoubleBooked = _context.Bookings.Find(b => b.VenueId == booking.VenueId && b.BookingDate == booking.BookingDate).Any();
                if (isDoubleBooked)
                {
                    ModelState.AddModelError("", "The venue is already booked for the selected date.");
                    return View(booking);
                }

                _context.Bookings.InsertOne(booking);
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }

        public IActionResult Delete(int id)
        {
            var booking = _context.Bookings.Find(b => b.BookingId == id).FirstOrDefault();
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _context.Bookings.DeleteOne(b => b.BookingId == id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while deleting the booking: {ex.Message}");
                return View();
            }
        }
    }
}