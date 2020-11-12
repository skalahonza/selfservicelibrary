using AutoMapper;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

using SelfServiceLibrary.API.Options;
using SelfServiceLibrary.BL.DTO.Issue;
using SelfServiceLibrary.BL.Entities;

using System;
using System.Threading.Tasks;

namespace SelfServiceLibrary.API.Services
{
    public class IssueService
    {
        private readonly IMongoCollection<Issue> _issues;
        private readonly IMongoCollection<Book> _books;
        private readonly IMapper _mapper;

        public IssueService(IOptions<MongoDbOptions> options, IMongoClient client, IMapper mapper)
        {
            var database = client.GetDatabase(options.Value.DatabaseName);
            _issues = database.GetCollection<Issue>(Issue.COLLECTION_NAME);
            _books = database.GetCollection<Book>(Book.COLLECTION_NAME);
            _mapper = mapper;
        }

        public async Task<IssueDetailDTO> Borrow(IssueCreateDTO issue, string username)
        {
            var entity = new Issue { Id = Guid.NewGuid() };
            entity = _mapper.Map(issue, entity);
            await _issues.InsertOneAsync(entity);
            var update = Builders<Book>.Update.AddToSet(x => x.Issues, entity.Id);
            await _books.UpdateOneAsync(x => x.Id == issue.BookId, update);
            return _mapper.Map<IssueDetailDTO>(entity);
        }

        public async Task<IssueDetailDTO?> Return(Guid id)
        {
            var update = Builders<Issue>.Update.Set(x => x.ReturnDate, DateTime.UtcNow);
            var result = await _issues.UpdateOneAsync(x => x.Id == id, update);
            if (!result.IsAcknowledged) return null;
            return _mapper.Map<IssueDetailDTO>(_issues.Find(x => x.Id == id).FirstOrDefaultAsync());
        }
    }
}
