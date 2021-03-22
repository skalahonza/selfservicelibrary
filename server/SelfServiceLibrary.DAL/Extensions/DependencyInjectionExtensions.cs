
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using MongoDB.Driver;

using SelfServiceLibrary.DAL.Options;

namespace SelfServiceLibrary.DAL.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddMongoDbPersistence(this IServiceCollection services, IConfiguration configuration)
        {
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
