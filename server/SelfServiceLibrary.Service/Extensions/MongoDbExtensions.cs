using System.Linq;

using MongoDB.Driver.Linq;

using SelfServiceLibrary.Service.Interfaces;

namespace SelfServiceLibrary.Service.Extensions
{
    public static class MongoDbExtensions
    {
        public static IMongoQueryable<TDestination> ProjectTo<TSource, TDestination>(this IQueryable<TSource> query, IMapper mapper) =>
            mapper.ProjectTo<TSource, TDestination>(query);
    }
}
