using System.Collections.Generic;
using System.Linq;

using MongoDB.Driver.Linq;

using SelfServiceLibrary.BL.Interfaces;

namespace SelfServiceLibrary.BL.Extensions
{
    public static class MongoDbExtensions
    {
        public static IMongoQueryable<TDestination> ProjectTo<TSource, TDestination>(this IQueryable<TSource> query, IMapper mapper) =>
            mapper.ProjectTo<TSource, TDestination>(query);

        public static IMongoQueryable<TSource> AsMongoDbQueryable<TSource>(this IQueryable<TSource> query) =>
            (IMongoQueryable<TSource>)query;

        public static async IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(this IMongoQueryable<TSource> source)
        {
            var asyncCursor = await source.ToCursorAsync();
            while (await asyncCursor.MoveNextAsync())
            {
                foreach (var current in asyncCursor.Current)
                {
                    yield return current;
                }
            }
        }
    }
}
