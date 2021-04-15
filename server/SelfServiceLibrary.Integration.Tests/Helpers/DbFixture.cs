using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Driver;

using Xunit;

namespace SelfServiceLibrary.Integration.Tests.Helpers
{
    public class DbFixture : IAsyncLifetime
    {
        public const string DB = "mongodb://root:rootpassword@localhost:27017";

        public Task InitializeAsync() =>
            Task.CompletedTask;

        public Task DisposeAsync()
        {
            var client = new MongoClient(DB);
            return client.DropDatabaseAsync(DbName);
        }

        public string DbName { get; } = $"test_db_{Guid.NewGuid()}";
    }
}
