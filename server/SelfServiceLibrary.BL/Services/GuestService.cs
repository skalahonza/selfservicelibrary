
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

using SelfServiceLibrary.BL.DTO.Guest;
using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.BL.Extensions;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.DAL;
using SelfServiceLibrary.DAL.Entities;
using SelfServiceLibrary.DAL.Queries;

namespace SelfServiceLibrary.BL.Services
{
    public class GuestService
    {
        private readonly MongoDbContext _dbContext;
        private readonly IMapper _mapper;

        public GuestService(MongoDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public Task<List<GuestDTO>> GetAll() =>
            _dbContext
                .Guests
                .AsQueryable()
                .ProjectTo<Guest, GuestDTO>(_mapper)
                .ToListAsync();

        public Task<List<UserInfoDTO>> Suggest(string term, int limit = 10) =>
            _dbContext
                .Guests
                .AsQueryable()
                .Take(limit)
                .Search(term)
                .ProjectTo<Guest, UserInfoDTO>(_mapper)
                .ToListAsync();

        public Task Add(GuestDTO guest)
        {
            var entity = _mapper.Map<Guest>(guest);
            entity.Id = Guid.NewGuid().ToString();
            return _dbContext.Guests.InsertOneAsync(entity);
        }

        public Task Update(GuestDTO guest) =>
            _dbContext
                .Guests
                .ReplaceOneAsync(x => x.Id == guest.Id, _mapper.Map<Guest>(guest));

        public Task Delete(string id) =>
            _dbContext
                .Guests
                .DeleteOneAsync(x => x.Id == id);
    }
}
