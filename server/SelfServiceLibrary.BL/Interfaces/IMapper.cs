using System.Linq;

using MongoDB.Driver.Linq;

namespace SelfServiceLibrary.BL.Interfaces
{
    public interface IMapper
    {
        //
        // Summary:
        //     Execute a mapping from the source object to a new destination object. The source
        //     type is inferred from the source object.
        //
        // Parameters:
        //   source:
        //     Source object to map from
        //
        // Type parameters:
        //   TDestination:
        //     Destination type to create
        //
        // Returns:
        //     Mapped destination object
        TDestination Map<TDestination>(object source);
        //
        // Summary:
        //     Execute a mapping from the source object to a new destination object.
        //
        // Parameters:
        //   source:
        //     Source object to map from
        //
        // Type parameters:
        //   TSource:
        //     Source type to use, regardless of the runtime type
        //
        //   TDestination:
        //     Destination type to create
        //
        // Returns:
        //     Mapped destination object
        TDestination Map<TSource, TDestination>(TSource source);
        //
        // Summary:
        //     Execute a mapping from the source object to the existing destination object.
        //
        // Parameters:
        //   source:
        //     Source object to map from
        //
        //   destination:
        //     Destination object to map into
        //
        // Type parameters:
        //   TSource:
        //     Source type to use
        //
        //   TDestination:
        //     Destination type
        //
        // Returns:
        //     The mapped destination object, same instance as the destination object
        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);

        /// <summary>
        /// Server side projection
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        IMongoQueryable<TDestination> ProjectTo<TSource, TDestination>(IQueryable<TSource> query);
    }
}
