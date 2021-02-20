using System.Collections.Generic;

using MongoDB.Bson.Serialization.Attributes;

namespace SelfServiceLibrary.Persistence.Entities
{
    public class User
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [BsonId]
        public string Username { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        #region Related entities
        public List<string> IssueIds { get; set; } = new List<string>();

        #endregion
    }
}
