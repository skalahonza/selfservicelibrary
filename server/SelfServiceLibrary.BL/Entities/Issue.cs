using System;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace SelfServiceLibrary.BL.Entities
{
    public class Issue
    {
        public const string COLLECTION_NAME = "issues";
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public string? BookName { get; set; }
        public string? ISBN { get; set; }
        public string? IssuedTo { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned { get; set; }
    }
}
