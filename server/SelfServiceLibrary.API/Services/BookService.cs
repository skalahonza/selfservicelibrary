using AutoMapper;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

using SelfServiceLibrary.API.Extensions;
using SelfServiceLibrary.API.Options;
using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.Entities;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SelfServiceLibrary.API.Services
{
    public class BookService
    {
        private readonly IMongoCollection<Book> _books;
        private readonly IMapper _mapper;

        public BookService(IOptions<MongoDbOptions> options, IMongoClient client, IMapper mapper)
        {
            var database = client.GetDatabase(options.Value.DatabaseName);
            _books = database.GetCollection<Book>(Book.COLLECTION_NAME);
            _mapper = mapper;
        }

        public Task<List<BookListDTO>> GetAll() =>
            _books
                .Find(Builders<Book>.Filter.Empty)
                .Project(Builders<Book>.Projection.Expression(x => _mapper.Map<BookListDTO>(x)))
                .ToListAsync();

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

        public async Task<BookDetailDTO?> Path(Guid id, BookEditDTO book)
        {
            var update = Builders<Book>
                .Update
                .SetIfNotNull(x => x.Name, book.Name)
                .SetIfNotNull(x => x.Quantity, book.Quantity);
            await _books.UpdateOneAsync(x => x.Id == id, update);
            return await GetDetail(id);
        }

        public async Task<bool> Delete(Guid id)
        {
            var result = await _books.DeleteOneAsync(x => x.Id == id);
            return result.DeletedCount != 0;
        }
    }
}
