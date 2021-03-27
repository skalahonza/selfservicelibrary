using System.Collections.Generic;
using System.Linq;

using MongoDB.Driver;

using SelfServiceLibrary.DAL.Entities;
using SelfServiceLibrary.DAL.Enums;

namespace SelfServiceLibrary.DAL.Queries
{
    public static class BookQueries
    {
        /// <summary>
        /// Librarian should see all books. Users without librarian role should only see Books where the visibility is set to true.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="userRoles">Roles the current user is member of</param>
        /// <returns></returns>
        public static IQueryable<Book> OnlyVisible(this IQueryable<Book> source, ISet<Role> userRoles) =>
            userRoles.Contains(Role.Librarian)
            ? source
            : source.Where(x => x.Status.IsVisible);

        /// <summary>
        /// Librarian should see all books. Users without librarian role should only see Books where the visibility is set to true.
        /// </summary>
        /// <param name="userRoles">Roles the current user is member of</param>
        /// <returns></returns>
        public static FilterDefinition<Book> OnlyVisible(this FilterDefinitionBuilder<Book> builder, ISet<Role> userRoles) =>
            userRoles.Contains(Role.Librarian)
            ? builder.Empty
            : builder.Eq(x => x.Status.IsVisible, true);
    }
}
