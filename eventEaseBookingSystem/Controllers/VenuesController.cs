using eventEaseBookingSystem.Data;
using eventEaseBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDB.Bson;
using System.IO;
using GridFSBucket = MongoDB.Driver.GridFS.GridFSBucket;

namespace eventEaseBookingSystem.Controllers
{
    public class VenuesController : Controller
    {
        private readonly MongoDbContext _context;
        private readonly GridFSBucket _gridFS;

        public VenuesController(MongoDbContext context)
        {
            _context = context;
            _gridFS = new GridFSBucket(_context.Database);
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
        public IActionResult Create(Venue venue, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var stream = imageFile.OpenReadStream())
                    {
                        var imageId = _gridFS.UploadFromStream(imageFile.FileName, stream);
                        venue.ImageId = imageId;
                    }
                }

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
        public IActionResult Edit(int id, Venue venue, IFormFile imageFile)
        {
            if (id != venue.VenueId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var stream = imageFile.OpenReadStream())
                    {
                        var imageId = _gridFS.UploadFromStream(imageFile.FileName, stream);
                        venue.ImageId = imageId;
                    }
                }

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

        public IActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction(nameof(Index));
            }

            var venues = _context.Venues.Find(v => v.VenueName.ToLower().Contains(query.ToLower()) || v.Location.ToLower().Contains(query.ToLower())).ToList();
            return View("Index", venues);
        }

        public async Task<IActionResult> GetImage(string id)
        {
            var imageId = ObjectId.Parse(id);
            var stream = await _gridFS.OpenDownloadStreamAsync(imageId);
            return File(stream, "image/jpeg");
        }
    }
}