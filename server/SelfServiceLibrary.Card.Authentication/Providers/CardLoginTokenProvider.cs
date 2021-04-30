
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using SelfServiceLibrary.Card.Authentication.Model;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SelfServiceLibrary.Card.Authentication.Options;

namespace SelfServiceLibrary.Card.Authentication.Providers
{
    public class CardLoginTokenProvider : DataProtectorTokenProvider<IdCard>
    {
        public const string NAME = "CardLoginTokenProvider";
        private readonly IMongoCollection<IdCard> _cards;

        public CardLoginTokenProvider(
            IDataProtectionProvider dataProtectionProvider,
            IOptions<DataProtectionTokenProviderOptions> options,
            IOptions<MongoDbDatabaseOptions> dbOptions,
            ILogger<DataProtectorTokenProvider<IdCard>> logger,
            IMongoClient client)
            : base(dataProtectionProvider, options, logger)
        {
            var database = client.GetDatabase(dbOptions.Value.DatabaseName);
            _cards = database.GetCollection<IdCard>(dbOptions.Value.CollectionName);
        }

        public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<IdCard> manager, IdCard user) =>
            Task.FromResult(false);

        public override async Task<string> GenerateAsync(string purpose, UserManager<IdCard> manager, IdCard user)
        {
            var token = await base.GenerateAsync(purpose, manager, user);
            await _cards.UpdateOneAsync(x => x.Id == user.Id, Builders<IdCard>.Update.AddToSet(x => x.Otps, token));
            return token;
        }

        public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<IdCard> manager, IdCard user)
        {
            if(await _cards.Find(x => x.Id == user.Id && x.Otps.Contains(token)).AnyAsync())
            {
                await _cards.UpdateOneAsync(x => x.Id == user.Id, Builders<IdCard>.Update.Pull(x => x.Otps, token));
                return true;
            }
            return false;
        }
    }
}
