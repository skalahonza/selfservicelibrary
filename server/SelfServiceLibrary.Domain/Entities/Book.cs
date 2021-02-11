using System;
using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace SelfServiceLibrary.Domain.Entities
{
    public class Book
    {
        public const string COLLECTION_NAME = "books";

        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; set; }

        /// <summary>
        /// Název knihy
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// První / hlavní autor, u sborníku většinou edito
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// Seznam dalších autorů 
        /// </summary>
        public List<string> CoAuthors { get; set; } = new List<string>();

        /// <summary>
        /// Číslo přiřazované ústřední knihovnou ČVUT (určuje typ knihy, stejné knihy mají stejné číslo)
        /// </summary>
        public string? SystemNumber { get; set; }

        public string? ISBN { get; set; }

        public string? ISSN { get; set; }

        public int Quantity { get; set; }

        public int Issued { get; set; }
    }
}
