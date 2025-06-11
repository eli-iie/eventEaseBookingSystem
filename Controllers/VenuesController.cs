using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eventEaseBookingSystem.Data;
using eventEaseBookingSystem.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace eventEaseBookingSystem.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "images";

        public VenuesController(ApplicationDbContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }        // GET: Venues
        public async Task<IActionResult> Index(string searchString, int? minCapacity, int? maxCapacity, bool? isAvailable)
        {
            var venues = from v in _context.Venues
                         select v;

            if (!String.IsNullOrEmpty(searchString))
            {
                venues = venues.Where(v => v.VenueName.Contains(searchString) 
                                        || v.Location.Contains(searchString));
            }

            if (minCapacity.HasValue)
            {
                venues = venues.Where(v => v.Capacity >= minCapacity.Value);
            }

            if (maxCapacity.HasValue)
            {
                venues = venues.Where(v => v.Capacity <= maxCapacity.Value);
            }

            if (isAvailable.HasValue)
            {
                venues = venues.Where(v => v.IsAvailable == isAvailable.Value);
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["MinCapacity"] = minCapacity;
            ViewData["MaxCapacity"] = maxCapacity;
            ViewData["IsAvailable"] = isAvailable;

            return View(await venues.ToListAsync());
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueId,VenueName,Location,Capacity,IsAvailable")] Venue venue, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload to Azure Blob Storage
                if (imageFile != null && imageFile.Length > 0)
                {
                    try
                    {
                        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                        var blobName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var blobClient = containerClient.GetBlobClient(blobName);

                        using var stream = imageFile.OpenReadStream();
                        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = imageFile.ContentType });

                        venue.ImageUrl = blobClient.Uri.ToString();
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Error uploading image: {ex.Message}");
                        return View(venue);
                    }
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        // POST: Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName,Location,Capacity,ImageUrl,IsAvailable")] Venue venue, IFormFile? imageFile)
        {
            if (id != venue.VenueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle image upload to Azure Blob Storage
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                        var blobName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var blobClient = containerClient.GetBlobClient(blobName);

                        using var stream = imageFile.OpenReadStream();
                        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = imageFile.ContentType });

                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(venue.ImageUrl))
                        {
                            try
                            {
                                var oldBlobName = Path.GetFileName(new Uri(venue.ImageUrl).LocalPath);
                                var oldBlobClient = containerClient.GetBlobClient(oldBlobName);
                                await oldBlobClient.DeleteIfExistsAsync();
                            }
                            catch
                            {
                                // Ignore errors when deleting old image
                            }
                        }

                        venue.ImageUrl = blobClient.Uri.ToString();
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId))
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
            return View(venue);
        }        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            // Check for related records and provide information
            var eventCount = await _context.Events.CountAsync(e => e.VenueId == id);
            var bookingCount = await _context.Bookings.CountAsync(b => b.VenueId == id);
            
            if (eventCount > 0)
            {
                ViewBag.EventCount = eventCount;
                ViewBag.HasEvents = true;
            }
            
            if (bookingCount > 0)
            {
                ViewBag.BookingCount = bookingCount;
                ViewBag.HasBookings = true;
            }

            return View(venue);
        }// POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                // Check if venue has events
                var hasEvents = await _context.Events.AnyAsync(e => e.VenueId == id);
                if (hasEvents)
                {
                    ModelState.AddModelError("", "Cannot delete venue because it has associated events. Please delete all events at this venue first.");
                    return View(venue);
                }

                // Check if venue has active bookings
                var hasBookings = await _context.Bookings.AnyAsync(b => b.VenueId == id);
                if (hasBookings)
                {
                    ModelState.AddModelError("", "Cannot delete venue with active bookings. Please cancel all bookings first.");
                    return View(venue);
                }

                // Delete image from Azure Blob Storage
                if (!string.IsNullOrEmpty(venue.ImageUrl))
                {
                    try
                    {
                        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                        var blobName = Path.GetFileName(new Uri(venue.ImageUrl).LocalPath);
                        var blobClient = containerClient.GetBlobClient(blobName);
                        await blobClient.DeleteIfExistsAsync();
                    }
                    catch
                    {
                        // Ignore errors when deleting image
                    }
                }

                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueId == id);
        }
    }
}
