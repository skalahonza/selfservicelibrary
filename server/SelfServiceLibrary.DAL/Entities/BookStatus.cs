using MongoDB.Bson.Serialization.Attributes;

namespace SelfServiceLibrary.DAL.Entities
{
    public class BookStatus
    {
        [BsonId]
        public string Name { get; set; } = "Default";

        public bool IsVissible { get; set; } = true;

        public bool CanBeBorrowed { get; set; } = true;
    }
}
