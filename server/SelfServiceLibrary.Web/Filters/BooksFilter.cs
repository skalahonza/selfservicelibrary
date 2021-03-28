using System.Collections.Generic;

using SelfServiceLibrary.DAL.Enums;
using SelfServiceLibrary.DAL.Filters;

namespace SelfServiceLibrary.Web.Filters
{
    public record BooksFilter : IBooksFilter
    {
        public BooksFilter(string publicationType) =>
            PublicationType = publicationType;

        public string? Departmentnumber { get; set; }
        public string? Name { get; set; }
        public string? Author { get; set; }
        public string? Status { get; set; }
        public bool? IsAvailable { get; set; }
        public string? PublicationType { get; set; }
        public ISet<Role> UserRoles { get; set; } = new HashSet<Role>();
        public bool? IsVisible => UserRoles.Contains(Role.Librarian) ? null : true;
    }
}
