using Newtonsoft.Json;

using System.Collections.Generic;

namespace SelfServiceLibrary.BL.Model
{
    public class UserContext
    {
        [JsonProperty("username")]
        public string? Username { get; set; }

        [JsonProperty("personalNumber")]
        public int? PersonalNumber { get; set; }

        [JsonProperty("kosPersonId")]
        public int? KosPersonId { get; set; }

        [JsonProperty("firstName")]
        public string? FirstName { get; set; }

        [JsonProperty("lastName")]
        public string? LastName { get; set; }

        [JsonProperty("fullName")]
        public string? FullName { get; set; }

        [JsonProperty("emails")]
        public IReadOnlyList<string>? Emails { get; set; }

        [JsonProperty("preferredEmail")]
        public string? PreferredEmail { get; set; }

        [JsonProperty("departments")]
        public IReadOnlyList<Department>? Departments { get; set; }

        [JsonProperty("rooms")]
        public IReadOnlyList<string>? Rooms { get; set; }

        [JsonProperty("phones")]
        public IReadOnlyList<object>? Phones { get; set; }

        [JsonProperty("roles")]
        public IReadOnlyList<string>? Roles { get; set; }

        [JsonProperty("technicalRoles")]
        public IReadOnlyList<string>? TechnicalRoles { get; set; }
    }
}
