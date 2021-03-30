using System.Collections.Generic;

using MongoDB.Bson.Serialization.Attributes;

using SelfServiceLibrary.DAL.Enums;

namespace SelfServiceLibrary.DAL.Entities
{
    public class User
    {
#pragma warning disable CS8618 // Database id cannot be empty
        [BsonId]
        public string Username { get; set; }
#pragma warning restore CS8618 // Database id cannot be empty
        public HashSet<Role> Roles { get; set; } = new HashSet<Role> { Role.Visitor };

        #region Related entities
        public List<IdCard> Cards { get; set; } = new List<IdCard>();
        #endregion
    }

    /// <summary>
    /// Sub-entity holding info about a user
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// Uživatelské jméno - může být prázdné pro lidi mimo ČVUT
        /// </summary>
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }

        public override string ToString() =>
            $"{FirstName} {LastName} ({Username})";
    }
}
