using System.Collections.Generic;
using System.Linq;

using MongoDB.Driver;

using SelfServiceLibrary.DAL.Entities;
using SelfServiceLibrary.DAL.Enums;
using SelfServiceLibrary.DAL.Filters;

namespace SelfServiceLibrary.DAL.Queries
{
    public static class BookQueries
    {
        public static IQueryable<Book> Filter(this IQueryable<Book> query, IBooksFilter filter)
        {
            if (filter.IsVisible.HasValue)
            {
                query = query.Where(x => x.Status.IsVisible == filter.IsVisible.Value);
            }

            if (!string.IsNullOrEmpty(filter.Departmentnumber))
            {
                query = query.Where(x => x.DepartmentNumber.ToLower().Contains(filter.Departmentnumber));
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(x => x.Name.ToLower().Contains(filter.Name));
            }

            if (!string.IsNullOrEmpty(filter.Author))
            {
                query = query.Where(x => x.Author.ToLower().Contains(filter.Author));
            }

            if (filter.IsAvailable.HasValue)
            {
                query = query.Where(x => x.IsAvailable == filter.IsAvailable.Value);
            }

            if (!string.IsNullOrEmpty(filter.PublicationType))
            {
                query = query.Where(x => x.PublicationType == filter.PublicationType);
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                query = query.Where(x => x.Status.Name == filter.Status);
            }

            return query;
        }

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
