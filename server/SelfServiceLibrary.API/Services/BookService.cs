using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.Extensions.Options;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

using SelfServiceLibrary.API.Extensions;
using SelfServiceLibrary.API.Options;
using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.Entities;

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
            _books = database.GetCollection<Book>("books");
            _mapper = mapper;
        }

        public Task<List<BookListDTO>> GetAll() =>
            _books
                .Find(Builders<Book>.Filter.Empty)
                .Project(Builders<Book>.Projection.Expression(x => _mapper.Map<BookListDTO>(x)))
                .ToListAsync();

       
    }
}
