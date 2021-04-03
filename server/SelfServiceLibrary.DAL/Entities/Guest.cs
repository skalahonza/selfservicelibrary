
using MongoDB.Bson.Serialization.Attributes;

namespace SelfServiceLibrary.DAL.Entities
{
    /// <summary>
    /// Holds data about persons outside of CTU (guests)
    /// </summary>
    public class Guest
    {
#pragma warning disable CS8618 // Database id cannot be empty
        [BsonId]
        public string Id { get; set; }
#pragma warning restore CS8618 // Database id cannot be empty
        public string? TitleBefore { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? TitleAfter { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
