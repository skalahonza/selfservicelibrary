using MongoDB.Bson.Serialization.Attributes;

namespace SelfServiceLibrary.DAL.Entities
{
    public class BookStatus
    {
        public const string DefaultName = "Default";

        [BsonId]
        public string Name { get; set; } = DefaultName;

        public bool IsVisible { get; set; } = true;

        public bool CanBeBorrowed { get; set; } = true;
    }
}
