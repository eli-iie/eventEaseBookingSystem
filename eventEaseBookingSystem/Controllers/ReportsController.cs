using eventEaseBookingSystem.Data;
using eventEaseBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using System.Linq;
using System.Diagnostics;

namespace eventEaseBookingSystem.Controllers
{
    public class ReportsController : Controller
    {
        private readonly MongoDbContext _context;

        public ReportsController(MongoDbContext context)
        {
            _context = context;
        }

        public IActionResult BookingSummary()
        {
            try
            {
                var pipeline = new[]
                {
                    new BsonDocument("$group", new BsonDocument
                    {
                        { "_id", "$VenueId" },
                        { "TotalBookings", new BsonDocument("$sum", 1) }
                    })
                };

                var reportCursor = _context.Bookings.Aggregate<BsonDocument>(pipeline);
                var report = new List<BsonDocument>();
                while (reportCursor.MoveNext())
                {
                    report.AddRange(reportCursor.Current);
                }

                return View(report);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog or NLog)
                Console.WriteLine($"Error in BookingSummary: {ex.Message}");

                // Optionally, return an error view or a meaningful message to the user
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
}