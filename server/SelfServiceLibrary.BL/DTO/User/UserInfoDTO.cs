using System;
using System.Collections.Generic;
using System.Linq;

namespace SelfServiceLibrary.BL.DTO.User
{
    public class UserInfoDTO : IComparable, IComparable<UserInfoDTO>
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
        /// <summary>
        /// Is empty for people inside CTU
        /// </summary>
        public string? PhoneNumber { get; set; }
        public bool IsGuest => !string.IsNullOrEmpty(GuestId);

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

        public int CompareTo(UserInfoDTO? other) =>
            (ToString() ?? string.Empty).CompareTo(other?.ToString() ?? string.Empty);

        public int CompareTo(object? obj)
        {
            if (obj is UserInfoDTO other)
                return CompareTo(other);
            else
                return 0;
        }
    }
}
