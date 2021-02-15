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
                .Find(Builders<Book>.Filter.Empty)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .Project(Builders<Book>.Projection.Expression(x => _mapper.Map<BookListDTO>(x)))
                .ToListAsync();

        public Task<long> GetTotalCount() =>
            _books.EstimatedDocumentCountAsync();

        public async Task<BookDetailDTO?> GetDetail(string departmentNumber) =>
            _mapper.Map<BookDetailDTO>(await _books
                .Find(x => x.DepartmentNumber == departmentNumber)
                .FirstOrDefaultAsync());

        public async Task<BookDetailDTO> Add(BookAddDTO book)
        {
            var entity = _mapper.Map<Book>(book);
            entity.Entered = DateTime.UtcNow;
            await _books.InsertOneAsync(entity);
            return _mapper.Map<BookDetailDTO>(entity);
        }

        public async Task ImportCsv(Stream csv)
        {
            await foreach (var book in _csv.ImportBooks(csv))
            {
                await _books.ReplaceOneAsync(x => x.DepartmentNumber == book.DepartmentNumber, book, new ReplaceOptions { IsUpsert = true });
            }
        }

        public async Task<bool> Delete(string departmentNumber)
        {
            var result = await _books.DeleteOneAsync(x => x.DepartmentNumber == departmentNumber);
            return result.DeletedCount != 0;
        }

        public Task DeleteAll() =>
            _books.DeleteManyAsync(Builders<Book>.Filter.Empty);
    }
}
