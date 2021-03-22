
using MongoDB.Driver;

using System;
using System.Linq.Expressions;

namespace SelfServiceLibrary.DAL.Extensions
{
    public static class MongoDbExtensions
    {
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
