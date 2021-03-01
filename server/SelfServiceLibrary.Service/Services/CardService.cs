using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

using SelfServiceLibrary.Persistence.Entities;
using SelfServiceLibrary.Persistence.Options;
using SelfServiceLibrary.Service.DTO.Card;
using SelfServiceLibrary.Service.Interfaces;

namespace SelfServiceLibrary.Service.Services
{
    public class CardService : ICardService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMapper _mapper;

        public CardService(IOptions<MongoDbOptions> options, IMongoClient client, IMapper mapper)
        {
            var database = client.GetDatabase(options.Value.DatabaseName);
            _users = database.GetCollection<User>(User.COLLECTION_NAME);
            _mapper = mapper;
        }

        public async Task<bool> Add(string username, AddCardDTO card)
        {
            var toAdd = _mapper.Map<IdCard>(card);
            var result = await _users
                .UpdateOneAsync(x => x.Username == username,
                Builders<User>.Update.AddToSet(x => x.Cards, toAdd),
                new UpdateOptions { IsUpsert = true });
            return result.ModifiedCount == 1;
        }

        public async Task<List<CardListDTO>> GetAll(string username)
        {
            var cards = await _users
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

            var result = await _users
                .UpdateOneAsync(filter,
                Builders<User>.Update.PullFilter(x => x.Cards, x => x.Number == cardNumber));

            return result.ModifiedCount == 1;
        }
    }
}
