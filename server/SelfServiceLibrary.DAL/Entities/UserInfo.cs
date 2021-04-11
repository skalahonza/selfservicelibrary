namespace SelfServiceLibrary.DAL.Entities
{
    /// <summary>
    /// Weak entity holding info about a person
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// Is empty for people outside CTU
        /// </summary>
        public string? Username { get; set; }
        /// <summary>
        /// Is empty for people inside CTU. Not empty for guests.
        /// </summary>
        public string? GuestId { get; set; }
        public string? TitleBefore { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? TitleAfter { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
