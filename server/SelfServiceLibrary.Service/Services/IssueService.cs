
using Microsoft.Extensions.Options;

using MongoDB.Bson;
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
        private readonly IMongoCollection<User> _users;
        private readonly IMapper _mapper;

        public IssueService(IOptions<MongoDbOptions> options, IMongoClient client, IMapper mapper)
        {
            var database = client.GetDatabase(options.Value.DatabaseName);
            _issues = database.GetCollection<Issue>(Issue.COLLECTION_NAME);
            _books = database.GetCollection<Book>(Book.COLLECTION_NAME);
            _users = database.GetCollection<User>(User.COLLECTION_NAME);
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
            var book = await _books.Find(x => x.DepartmentNumber == departmentNumber).FirstOrDefaultAsync();
            if (book == null)
            {
                // TODO handle not found                
            }

            // try to mark the book as borrowed
            var result = await _books.UpdateOneAsync(
                x => x.DepartmentNumber == departmentNumber && x.IsAvailable,
                Builders<Book>.Update.Set(x => x.IsAvailable, false));
            if (result.ModifiedCount == 0)
            {
                // TODO handle book was already taken
            }

            // create issue document
            var entity = new Issue
            {
                Id = Guid.NewGuid().ToString(),
                IssueDate = DateTime.UtcNow,
                IssuedTo = username
            };
            entity = _mapper.Map(issue, entity);
            entity = _mapper.Map(book, entity);
            await _issues.InsertOneAsync(entity);

            // link issue to a user
            await _users.UpdateOneAsync(
                x => x.Username == username,
                Builders<User>.Update.AddToSet(x => x.IssueIds, entity.Id),
                new UpdateOptions { IsUpsert = true });

            // link issue to a book
            await _books.UpdateOneAsync(
                x => x.DepartmentNumber == departmentNumber,
                Builders<Book>.Update.AddToSet(x => x.IssueIds, entity.Id));

            return _mapper.Map<IssueDetailDTO>(entity);
        }

        /// <summary>
        /// Return a borrowed book
        /// </summary>
        /// <param name="id">Id of the issue document</param>
        /// <returns></returns>
        public async Task<IssueDetailDTO?> Return(string id)
        {
            await _issues.UpdateOneAsync(
                x => x.Id == id,
                Builders<Issue>
                    .Update
                    .Set(x => x.IsReturned, true)
                    .CurrentDate(x => x.ReturnDate));

            var issue = await _issues.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (issue == null)
            {
                // TODO handle not found
            }
            return _mapper.Map<IssueDetailDTO>(issue);
        }
    }
}
