using MongoDB.Bson.Serialization.Attributes;

namespace SelfServiceLibrary.Persistence.Entities
{
    public class BookStatus
    {
        public const string COLLECTION_NAME = "bookStatuses";

        [BsonId]
        public string Name { get; set; } = "Default";

        public bool IsVissible { get; set; } = true;

        public bool CanBeBorrowed { get; set; } = true;
    }
}
