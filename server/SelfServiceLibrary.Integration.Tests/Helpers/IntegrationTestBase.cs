using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.BL.Services;
using SelfServiceLibrary.CSV;
using SelfServiceLibrary.DAL.Extensions;
using SelfServiceLibrary.Mapping;
using SelfServiceLibrary.Mapping.Profiles;

using Xunit;

namespace SelfServiceLibrary.Integration.Tests.Helpers
{
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        protected IConfiguration Configuration;

        protected IServiceCollection Services = new ServiceCollection();

        protected readonly DbFixture Fixture;

        protected IntegrationTestBase(DbFixture fixture)
        {
            Fixture = fixture;

            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<IntegrationTestBase>()
                .AddEnvironmentVariables()
                .AddInMemoryCollection(new[] {
                    new KeyValuePair<string,string>("MongoDb:ConnectionString", DbFixture.DB),
                    new KeyValuePair<string,string>("Identity:ConnectionString", $"{DbFixture.DB}/{fixture.DbName}?authSource=admin"),
                    new KeyValuePair<string,string>("MongoDb:DatabaseName", fixture.DbName),
                    new KeyValuePair<string,string>("SendGrid:ApiKey", "apikey"),
                })
                .Build();

            // Logging
            Services.AddLogging();

            // Mapping
            Services.AddAutoMapper(typeof(BookProfile));
            Services.AddScoped<IMapper, AutoMapperAdapter>();

            // Persistence, MongoDB
            Services.AddMongoDbPersistence(Configuration.GetSection("MongoDb"));

            // Authorization
            Services.AddSingleton<IAuthorizationContext, PermissiveContext>();

            // CSV
            Services.AddScoped<ICsvService, CsvHelperAdapter>();

            // Business logic
            Services.AddScoped<IBookService, BookService>();
            Services.AddScoped<IIssueService, IssueService>();
            Services.AddScoped<IUserService, UserService>();

            // Notifications
            Services.AddSingleton<INotificationService, NullNotificationService>();
        }

        public Task InitializeAsync()
        {
            return Fixture.Seed(Services);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
