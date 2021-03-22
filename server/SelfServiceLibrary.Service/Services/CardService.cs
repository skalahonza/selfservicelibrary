using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Driver;

using SelfServiceLibrary.BL.DTO.Card;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.DAL;
using SelfServiceLibrary.DAL.Entities;

namespace SelfServiceLibrary.BL.Services
{
    public class CardService : ICardService
    {
        private readonly MongoDbContext _dbContext;
        private readonly IMapper _mapper;

        public CardService(MongoDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> Add(string username, AddCardDTO card)
        {
            var toAdd = _mapper.Map<IdCard>(card);
            var result = await _dbContext
                .Users
                .UpdateOneAsync(x => x.Username == username,
                Builders<User>.Update.AddToSet(x => x.Cards, toAdd),
                new UpdateOptions { IsUpsert = true });
            return result.ModifiedCount == 1;
        }

        public async Task<List<CardListDTO>> GetAll(string username)
        {
            var cards = await _dbContext
                .Users
                .Find(x => x.Username == username)
                .Project(x => x.Cards)
                .FirstOrDefaultAsync();

            return _mapper.Map<List<CardListDTO>>(cards);
        }

        public async Task<bool> Remove(string username, string cardNumber)
        {
            // The & operator is overloaded. Other overloaded operators include the | operator for “or” and the ! operator for “not”.
            var builder = Builders<User>.Filter;
            var filter = builder.Eq(x => x.Username, username) & builder.ElemMatch(x => x.Cards, x => x.Number == cardNumber);

            var result = await _dbContext
                .Users
                .UpdateOneAsync(filter,
                Builders<User>.Update.PullFilter(x => x.Cards, x => x.Number == cardNumber));

            return result.ModifiedCount == 1;
        }
    }
}
