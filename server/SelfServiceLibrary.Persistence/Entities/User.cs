using System;
using System.Collections.Generic;

namespace SelfServiceLibrary.Persistence.Entities
{
    public class User
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Username { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public List<Guid> Issues { get; set; } = new List<Guid>();
    }
}
