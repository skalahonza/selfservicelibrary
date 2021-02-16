using Microsoft.Extensions.Options;

using MongoDB.Driver;

using SelfServiceLibrary.Persistence.Entities;
using SelfServiceLibrary.Persistence.Options;
using SelfServiceLibrary.Service.DTO.Book;
using SelfServiceLibrary.Service.Extensions;
using SelfServiceLibrary.Service.Interfaces;

using System;
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

        public async Task<BookDetailDTO?> GetDetail(Guid id) =>
            _mapper.Map<BookDetailDTO>(await _books
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync());

        public async Task<BookDetailDTO> Add(BookAddDTO book)
        {
            var entity = new Book { Id = Guid.NewGuid() };
            entity = _mapper.Map(book, entity);
            await _books.InsertOneAsync(entity);
            return _mapper.Map<BookDetailDTO>(entity);
        }

        public async Task ImportCsv(Stream csv)
        {
            await foreach (var book in _csv.ImportBooks(csv))
            {
                await _books.InsertOneAsync(book);
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            var result = await _books.DeleteOneAsync(x => x.Id == id);
            return result.DeletedCount != 0;
        }

        public Task DeleteAll() =>
            _books.DeleteManyAsync(Builders<Book>.Filter.Empty);
    }
}
