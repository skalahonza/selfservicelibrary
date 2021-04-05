using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace SelfServiceLibrary.BL.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<TSource> Sort<TSource>(this IQueryable<TSource> query, IEnumerable<(string column, ListSortDirection direction)>? sortings)
        {
            if(sortings == null)
            {
                return query;
            }

            // https://dotnetfiddle.net/GdxsMG
            var columns = sortings.Select(x =>
            {
                var (column, direction) = x;
                return direction switch
                {
                    ListSortDirection.Ascending => column,
                    ListSortDirection.Descending => column + " DESC",
                    _ => throw new NotImplementedException(),
                };
            });
            var expression = string.Join(", ", columns);
            if (string.IsNullOrEmpty(expression))
            {
                return query;
            }
            else
            {
                return query.OrderBy(expression);
            }
        }
    }
}
