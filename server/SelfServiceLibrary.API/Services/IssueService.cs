using AutoMapper;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

using SelfServiceLibrary.API.Options;
using SelfServiceLibrary.BL.Entities;

namespace SelfServiceLibrary.API.Services
{
    public class IssueService
    {
        private readonly IMongoCollection<Issue> _issues;
        private readonly IMapper _mapper;

        public IssueService(IOptions<MongoDbOptions> options, IMongoClient client, IMapper mapper)
        {
            var database = client.GetDatabase(options.Value.DatabaseName);
            _issues = database.GetCollection<Issue>("issues");
            _mapper = mapper;
        }
    }
}
