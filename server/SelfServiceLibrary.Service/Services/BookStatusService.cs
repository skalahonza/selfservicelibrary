﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

using SelfServiceLibrary.Persistence;
using SelfServiceLibrary.Persistence.Entities;
using SelfServiceLibrary.Persistence.Options;
using SelfServiceLibrary.Service.DTO.BookStatus;
using SelfServiceLibrary.Service.Extensions;
using SelfServiceLibrary.Service.Interfaces;

namespace SelfServiceLibrary.Service.Services
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
