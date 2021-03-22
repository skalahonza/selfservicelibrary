﻿using System.Collections.Generic;

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
        public List<string> IssueIds { get; set; } = new List<string>();
        public List<IdCard> Cards { get; set; } = new List<IdCard>();
        #endregion
    }
}