using System.Collections.Generic;

namespace CVUT.Usermap.Model
{
    public class User
    {
        public string? Username { get; set; }

        public int? PersonalNumber { get; set; }

        public int? KosPersonId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? FullName { get; set; }

        public IReadOnlyList<string> Emails { get; set; } = new List<string>();

        public string? PreferredEmail { get; set; }

        public IReadOnlyList<Department> Departments { get; set; } = new List<Department>();

        public IReadOnlyList<string> Rooms { get; set; } = new List<string>();

        public IReadOnlyList<string> Phones { get; set; } = new List<string>();

        public IReadOnlyList<string> Roles { get; set; } = new List<string>();

        public IReadOnlyList<string> TechnicalRoles { get; set; } = new List<string>();
    }
}
