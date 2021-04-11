using System.Linq;

using SelfServiceLibrary.DAL.Entities;
using SelfServiceLibrary.DAL.Filters;

namespace SelfServiceLibrary.DAL.Queries
{
    public static class IssueQueries
    {
        public static IQueryable<Issue> Filter(this IQueryable<Issue> query, IIssuesFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Book))
            {
                query = query.Where(x => 
                    x.DepartmentNumber!.ToLower().Contains(filter.Book) ||
                    x.BookName!.ToLower().Contains(filter.Book)
                );
            }

            if (!string.IsNullOrEmpty(filter.IssuedTo))
            {
                query = query.Where(x =>
                    x.IssuedTo.Username == filter.IssuedTo ||
                    x.IssuedTo.GuestId == filter.IssuedTo
                );
            }

            if (!string.IsNullOrEmpty(filter.IssuedBy))
            {
                query = query.Where(x => x.IssuedBy.Username == filter.IssuedBy);
            }

            if (!string.IsNullOrEmpty(filter.ReturnedBy))
            {
                query = query.Where(x => x.ReturnedBy!.Username == filter.ReturnedBy);
            }

            if (filter.IsReturned.HasValue)
            {
                query = query.Where(x => x.IsReturned == filter.IsReturned.Value);
            }

            return query;
        }
    }
}
