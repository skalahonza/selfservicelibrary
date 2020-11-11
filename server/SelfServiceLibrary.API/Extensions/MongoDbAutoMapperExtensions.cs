using AutoMapper;
using AutoMapper.QueryableExtensions;

using MongoDB.Driver.Linq;

using System;

namespace SelfServiceLibrary.API.Extensions
{

    public static class MongoDbAutoMapperExtensions
    {
        public static IMongoQueryable<TSource> MapTo<TSource, TDestination>(this IMongoQueryable<TSource> source, IConfigurationProvider configuration) =>
            source
                .ProjectTo<TDestination>(configuration)
                as IMongoQueryable<TSource> ?? throw new NotImplementedException("Automapper Project To does not implement IMongoQueryable<TDestination>.");
    }
}
