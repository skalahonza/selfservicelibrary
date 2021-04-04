namespace SelfServiceLibrary.DAL.Entities
{
    /// <summary>
    /// Weak entity holding info about a person
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// Can be empty for people outside CTU
        /// </summary>
        public string? Username { get; set; }
        public string? TitleBefore { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? TitleAfter { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
