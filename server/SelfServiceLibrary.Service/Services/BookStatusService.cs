using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Driver;

using SelfServiceLibrary.BL.DTO.BookStatus;
using SelfServiceLibrary.BL.Extensions;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.DAL;
using SelfServiceLibrary.DAL.Entities;

namespace SelfServiceLibrary.BL.Services
{
    public class BookStatusService
    {
        private readonly MongoDbContext _dbContext;
        private readonly IMapper _mapper;

        public BookStatusService(MongoDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public Task<List<BookStatusListDTO>> GetAll() =>
            _dbContext
                .BookStatuses
                .AsQueryable()
                .ProjectTo<BookStatus, BookStatusListDTO>(_mapper)
                .ToListAsync();

        public Task Create(BookStatusCreateDTO bookStatus) =>
            _dbContext.BookStatuses.InsertOneAsync(_mapper.Map<BookStatus>(bookStatus)); // TODO handle duplicity

        public async Task Update(string name, BookStatusUpdateDTO bookStatus)
        {
            var entity = _mapper.Map<BookStatus>(bookStatus);
            await _dbContext.BookStatuses.ReplaceOneAsync(x => x.Name == name, entity, new ReplaceOptions { IsUpsert = true });
            await _dbContext.Books.UpdateManyAsync(x => x.Status.Name == name, Builders<Book>.Update.Set(x => x.Status, entity));
        }

        public async Task Remove(string name)
        {
            await _dbContext.BookStatuses.DeleteOneAsync(x => x.Name == name);
            await _dbContext.Books.UpdateManyAsync(x => x.Status.Name == name, Builders<Book>.Update.Set(x => x.Status, new BookStatus()));
        }
    }
}
