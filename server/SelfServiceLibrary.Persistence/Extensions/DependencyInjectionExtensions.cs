
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using MongoDB.Bson.Serialization;
using MongoDB.Driver;

using SelfServiceLibrary.Persistence.Options;

namespace SelfServiceLibrary.Persistence.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddMongoDbPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            MongoDbContext.ConfigureTables();

            services
                .AddOptions<MongoDbOptions>()
                .Bind(configuration)
                .ValidateDataAnnotations();

            services.AddSingleton<IMongoClient, MongoClient>(x =>
            {
                var options = x.GetRequiredService<IOptions<MongoDbOptions>>();
                return new MongoClient(options.Value.ConnectionString);
            });
            services.AddScoped<MongoDbContext>();
            return services;
        }
    }
}
