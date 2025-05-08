using MongoDB.Driver;
using eventEaseBookingSystem.Models;
using System.Linq;

namespace eventEaseBookingSystem.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);

            // Ensure collections are created
            InitializeCollections();
        }

        public IMongoCollection<Venue> Venues => _database.GetCollection<Venue>("Venues");
        public IMongoCollection<Event> Events => _database.GetCollection<Event>("Events");
        public IMongoCollection<Booking> Bookings => _database.GetCollection<Booking>("Bookings");
        public IMongoDatabase Database => _database;

        private void InitializeCollections()
        {
            var collectionNames = _database.ListCollectionNames().ToList();

            if (!collectionNames.Contains("Venues"))
            {
                _database.CreateCollection("Venues");
            }

            if (!collectionNames.Contains("Events"))
            {
                _database.CreateCollection("Events");
            }

            if (!collectionNames.Contains("Bookings"))
            {
                _database.CreateCollection("Bookings");
            }
        }

        public void SeedDatabase()
        {
            var venuesCollection = _database.GetCollection<Venue>("Venues");

            if (!venuesCollection.Find(_ => true).Any())
            {
                var sampleVenues = new List<Venue>
                {
                    new Venue { VenueName = "Grand Hall", Location = "Downtown", Capacity = 500 },
                    new Venue { VenueName = "Conference Center", Location = "Uptown", Capacity = 300 },
                    new Venue { VenueName = "Open Air Theater", Location = "Suburbs", Capacity = 1000 }
                };

                venuesCollection.InsertMany(sampleVenues);
            }
        }
    }
}