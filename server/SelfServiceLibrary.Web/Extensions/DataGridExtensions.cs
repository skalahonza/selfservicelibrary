using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Blazorise;
using Blazorise.DataGrid;

namespace SelfServiceLibrary.Web.Extensions
{
    public static class DataGridExtensions
    {
        public static IEnumerable<(string column, ListSortDirection direction)> GetSortingCriteria<TItem>(this DataGridReadDataEventArgs<TItem> e) =>
            e.Columns
                .Where(x => x.Direction != SortDirection.None)
                .Select(x =>
                {
                    var column = x.Field;
                    var direction = x.Direction switch
                    {
                        SortDirection.Ascending => ListSortDirection.Ascending,
                        SortDirection.Descending => ListSortDirection.Descending,
                        _ => throw new System.NotImplementedException(),
                    };
                    return (column, direction);
                });
    }
}
