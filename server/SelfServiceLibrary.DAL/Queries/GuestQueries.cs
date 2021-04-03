using System.Linq;

using SelfServiceLibrary.DAL.Entities;

namespace SelfServiceLibrary.DAL.Queries
{
    public static class GuestQueries
    {
        public static IQueryable<Guest> Search(this IQueryable<Guest> query, string term) =>
            query.Where(guest =>
                guest.FirstName.ToLower().Contains(term) ||
                guest.LastName.ToLower().Contains(term)
            );
    }
}
