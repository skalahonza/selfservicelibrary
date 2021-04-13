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
            Guests = _database.GetCollection<Guest>("guests");
        }

        public IMongoCollection<Book> Books { get; }
        public IMongoCollection<BookStatus> BookStatuses { get; }
        public IMongoCollection<Issue> Issues { get; }
        public IMongoCollection<User> Users { get; }
        public IMongoCollection<Guest> Guests { get; }

        /// <summary>
        /// This method should only be called at application startup and is responsible for DB configuration (primary keys etc) and asserting that the indexes exist.
        /// </summary>
        /// <returns></returns>
        public async Task EnsureIndexesExist()
        {
            // Books
            var fullTextSearchIndex = Builders<Book>
                    .IndexKeys
                    .Text(x => x.Name)
                    .Text(x => x.Author)
                    .Text(x => x.CoAuthors)
                    .Text(x => x.Publisher)
                    .Text(x => x.CountryOfPublication)
                    .Text(x => x.Conference)
                    .Text(x => x.Keywords);
            
            await Books
                .Indexes                
                .CreateOneAsync(new CreateIndexModel<Book>(fullTextSearchIndex));

            // Issues
            var sortingIndex = Builders<Issue>
                .IndexKeys
                .Ascending(x => x.IsReturned)
                .Ascending(x => x.ExpiryDate);

            await Issues
                .Indexes
                .CreateOneAsync(new CreateIndexModel<Issue>(sortingIndex));
        }
    }
}
