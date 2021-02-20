using Microsoft.Extensions.Options;

using MongoDB.Driver;

using SelfServiceLibrary.Persistence.Entities;
using SelfServiceLibrary.Persistence.Options;
using SelfServiceLibrary.Service.DTO.Book;
using SelfServiceLibrary.Service.Extensions;
using SelfServiceLibrary.Service.Interfaces;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SelfServiceLibrary.Service.Services
{
    public class BookService
    {
        private readonly IMongoCollection<Book> _books;
        private readonly IMapper _mapper;
        private readonly ICsvImporter _csv;

        public BookService(IOptions<MongoDbOptions> options, IMongoClient client, IMapper mapper, ICsvImporter csv)
        {
            var database = client.GetDatabase(options.Value.DatabaseName);
            _books = database.GetCollection<Book>(Book.COLLECTION_NAME);
            _mapper = mapper;
            _csv = csv;
        }

        public Task<List<BookListDTO>> GetAll(int page, int pageSize) =>
            _books
                .AsQueryable()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<Book, BookListDTO>(_mapper)
                .ToListAsync();

        public Task<long> GetTotalCount() =>
            _books.EstimatedDocumentCountAsync();

        public async Task ImportCsv(Stream csv)
        {
            await foreach (var book in _csv.ImportBooks(csv))
            {
                await _books.InsertOneAsync(book);
            }
        }

        public Task DeleteAll() =>
            _books.DeleteManyAsync(Builders<Book>.Filter.Empty);
    }
}
