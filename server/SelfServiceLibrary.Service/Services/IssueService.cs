
using Microsoft.Extensions.Options;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

using SelfServiceLibrary.Persistence.Entities;
using SelfServiceLibrary.Persistence.Options;
using SelfServiceLibrary.Service.DTO.Issue;
using SelfServiceLibrary.Service.Extensions;
using SelfServiceLibrary.Service.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SelfServiceLibrary.Service.Services
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

        public Task<List<IssueListlDTO>> GetAll(string username) =>
            _issues.Find(Builders<Issue>.Filter.Where(x => x.IssuedTo == username))
                .Project(Builders<Issue>.Projection.Expression(x => _mapper.Map<IssueListlDTO>(x)))
                .ToListAsync();

        public Task<List<IssueListlDTO>> GetByIds(IEnumerable<string> ids) =>
            _issues
                .AsQueryable()
                .Where(x => ids.Contains(x.Id))
                .ProjectTo<Issue, IssueListlDTO>(_mapper)
                .ToListAsync();

        /// <summary>
        /// Borrow a book if available
        /// </summary>
        /// <param name="issue">Issue details.</param>
        /// <param name="username">To whom will the book be issued.</param>
        /// <param name="departmentNumber">Department number of the book to borrow.</param>
        /// <returns>Issue details.</returns>
        public async Task<IssueDetailDTO> Borrow(string username, string departmentNumber, IssueCreateDTO issue)
        {
            throw new NotImplementedException();
        }

        public async Task<IssueDetailDTO?> Return(string id)
        {
            throw new NotImplementedException();
        }
    }
}
