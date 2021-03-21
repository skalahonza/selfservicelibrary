using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using MongoDB.Bson.Serialization;
using MongoDB.Driver;

using SelfServiceLibrary.Persistence.Entities;
using SelfServiceLibrary.Persistence.Options;

namespace SelfServiceLibrary.Persistence
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbOptions> options, IMongoClient client)
        {
            _database = client.GetDatabase(options.Value.DatabaseName);
            Books = _database.GetCollection<Book>("books");
            BookStatuses = _database.GetCollection<BookStatus>("bookStatuses");
            Issues = _database.GetCollection<Issue>("issues");
            Users = _database.GetCollection<User>("users");
        }

        public IMongoCollection<Book> Books { get; }
        public IMongoCollection<BookStatus> BookStatuses { get; }
        public IMongoCollection<Issue> Issues { get; }
        public IMongoCollection<User> Users { get; }

        public static void ConfigureTables()
        {
            BsonClassMap.RegisterClassMap<Book>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.DepartmentNumber);
                cm.MapMember(c => c.TextMatchScore).SetIgnoreIfNull(true);
            });

            BsonClassMap.RegisterClassMap<BookStatus>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Name);
            });

            BsonClassMap.RegisterClassMap<User>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Username);
            });
        }

        /// <summary>
        /// This method should only be called at application startup and is responsible for DB configuration (primary keys etc) and asserting that the indexes exist.
        /// </summary>
        /// <returns></returns>
        public Task EnsureIndexesExist()
        {
            // indexes
            return Books
                .Indexes
                .CreateOneAsync(new CreateIndexModel<Book>(Builders<Book>
                    .IndexKeys
                    .Text("$**")));
        }
    }
}
