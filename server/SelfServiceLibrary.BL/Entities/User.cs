using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SelfServiceLibrary.BL.Entities
{
    public class User
    {
        [BsonId]
        public string? Username { get; set; }
        public List<ObjectId> Issues { get; set; } = new List<ObjectId>();
    }
}
