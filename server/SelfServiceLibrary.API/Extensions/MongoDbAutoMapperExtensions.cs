using AutoMapper;
using AutoMapper.QueryableExtensions;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

using System;
using System.Linq.Expressions;

namespace SelfServiceLibrary.API.Extensions
{

    public static class MongoDbAutoMapperExtensions
    {
        public static IMongoQueryable<TSource> MapTo<TSource, TDestination>(this IMongoQueryable<TSource> source, IConfigurationProvider configuration) =>
            source
                .ProjectTo<TDestination>(configuration)
                as IMongoQueryable<TSource> ?? throw new NotImplementedException("Automapper Project To does not implement IMongoQueryable<TDestination>.");

        public static UpdateDefinition<TDocument> SetIfNotNull<TDocument, TField>(this UpdateDefinitionBuilder<TDocument> builder, Expression<Func<TDocument, TField>> field, TField value) =>
            value == null
                ? builder.Combine()
                : builder.Set(field, value);

        public static UpdateDefinition<TDocument> SetIfNotNull<TDocument, TField>(this UpdateDefinition<TDocument> builder, Expression<Func<TDocument, TField>> field, TField value) =>
            value == null
                ? builder
                : builder.Set(field, value);
    }
}
