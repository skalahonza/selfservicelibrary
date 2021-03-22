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
    }
}
