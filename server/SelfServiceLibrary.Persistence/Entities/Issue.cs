using System;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace SelfServiceLibrary.Persistence.Entities
{
    public class Issue
    {
        public const string COLLECTION_NAME = "issues";

        /// <summary>
        /// Id v databázi
        /// </summary>
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; set; }

        /// <summary>
        /// Id knihy zapůjčené knihy
        /// </summary>
        public Guid BookId { get; set; }

        /// <summary>
        /// Id uživatele
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Název knihy
        /// </summary>
        public string? BookName { get; set; }

        public string? ISBN { get; set; }
        public string? IssuedTo { get; set; }

        /// <summary>
        /// Datum zapůjčení
        /// </summary>
        public DateTime? IssueDate { get; set; }

        /// <summary>
        /// Předpokládané datum vrácení
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Datum vrácení
        /// </summary>
        public DateTime? ReturnDate { get; set; }

        public bool IsReturned { get; set; }
    }
}
