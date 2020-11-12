using System;
using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace SelfServiceLibrary.BL.Entities
{
    public class Book
    {
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; set; }
        /// <summary>
        /// Název knihy
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// První / hlavní autor, u sborníku většinou editor 
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

        public List<ObjectId> Issues { get; set; } = new List<ObjectId>();
    }
}
