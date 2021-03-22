using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using MongoDB.Bson.Serialization;
using MongoDB.Driver;

using SelfServiceLibrary.DAL.Entities;
using SelfServiceLibrary.DAL.Options;

namespace SelfServiceLibrary.DAL
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
