using System.Collections.Generic;
using System.Linq;

namespace SelfServiceLibrary.BL.DTO.User
{
    public class UserInfoDTO
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
        /// <summary>
        /// Can be empty for people inside CTU
        /// </summary>
        public string? PhoneNumber { get; set; }
        public bool IsGuest => string.IsNullOrEmpty(Username);

        private IEnumerable<string?> NameFields()
        {
            yield return TitleBefore;
            yield return FirstName;
            yield return LastName;
            yield return TitleAfter;
        }

        public override string ToString()
        {
            var text = string.IsNullOrEmpty(FullName)
                ? string.Join(" ", NameFields().Where(x => !string.IsNullOrEmpty(x)))
                : FullName;
            return string.IsNullOrEmpty(Username)
                ? text
                : $"{text} ({Username})";
        }
    }
}
