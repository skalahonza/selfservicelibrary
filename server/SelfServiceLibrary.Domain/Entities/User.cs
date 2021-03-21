using System.Collections.Generic;

namespace SelfServiceLibrary.Persistence.Entities
{
    public class User
    {
#pragma warning disable CS8618 // Cannot be empty, enforced by validation
        public string Username { get; set; }
#pragma warning restore CS8618 // Cannot be empty, enforced by validation

        #region Related entities
        public List<string> IssueIds { get; set; } = new List<string>();
        public List<IdCard> Cards { get; set; } = new List<IdCard>();
        #endregion
    }
}
