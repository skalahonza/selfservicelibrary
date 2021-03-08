using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

using SelfServiceLibrary.Persistence.Entities;
using SelfServiceLibrary.Persistence.Options;
using SelfServiceLibrary.Service.DTO.BookStatus;
using SelfServiceLibrary.Service.Extensions;
using SelfServiceLibrary.Service.Interfaces;

namespace SelfServiceLibrary.Service.Services
{
    public class BookStatusService
    {
        private readonly IMongoCollection<BookStatus> _statuses;
        private readonly IMongoCollection<Book> _books;
        private readonly IMapper _mapper;

        public BookStatusService(IOptions<MongoDbOptions> options, IMongoClient client, IMapper mapper)
        {
            var database = client.GetDatabase(options.Value.DatabaseName);
            _statuses = database.GetCollection<BookStatus>(BookStatus.COLLECTION_NAME);
            _books = database.GetCollection<Book>(Book.COLLECTION_NAME);
            _mapper = mapper;
        }

        public Task<List<BookStatusListDTO>> GetAll() =>
            _statuses
                .AsQueryable()
                .ProjectTo<BookStatus, BookStatusListDTO>(_mapper)
                .ToListAsync();

        public Task Create(BookStatusCreateDTO bookStatus) =>
            _statuses.InsertOneAsync(_mapper.Map<BookStatus>(bookStatus)); // TODO handle duplicity

        public async Task Update(string name, BookStatusUpdateDTO bookStatus)
        {
            var entity = _mapper.Map<BookStatus>(bookStatus);
            await _statuses.ReplaceOneAsync(x => x.Name == name, entity, new ReplaceOptions { IsUpsert = true });
            await _books.UpdateManyAsync(x => x.Status.Name == name, Builders<Book>.Update.Set(x => x.Status, entity));
        }
    }
}
