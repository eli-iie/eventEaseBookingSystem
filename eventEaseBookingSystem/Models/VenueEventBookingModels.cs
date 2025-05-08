using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eventEaseBookingSystem.Models
{
    public class Venue
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public int VenueId { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public int VenueId { get; set; }
        public Venue Venue { get; set; } = new Venue();
    }

    public class Booking
    {
        public int BookingId { get; set; }
        public int EventId { get; set; }
        public int VenueId { get; set; }
        public DateTime BookingDate { get; set; }

        public Event Event { get; set; } = new Event();
        public Venue Venue { get; set; } = new Venue();
    }
}