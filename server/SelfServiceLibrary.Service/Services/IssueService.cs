
using Microsoft.Extensions.Options;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

using SelfServiceLibrary.Persistence.Entities;
using SelfServiceLibrary.Persistence.Options;
using SelfServiceLibrary.Service.DTO.Book;
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

        public Task<long> GetTotalCount() =>
            _issues.EstimatedDocumentCountAsync();

        public Task<List<IssueListlDTO>> GetAll(int page, int pageSize) =>
            _issues
                .AsQueryable()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<Issue, IssueListlDTO>(_mapper)
                .ToListAsync();

        public Task<List<IssueListlDTO>> GetAll(string username) =>
            _issues
                .AsQueryable()
                .Where(x => x.IssuedTo == username)
                .OrderBy(x => x.IsReturned).ThenBy(x => x.ExpiryDate)
                .ProjectTo<Issue, IssueListlDTO>(_mapper)
                .ToListAsync();

        public Task<List<IssueListlDTO>> GetAll(BookDetailDTO book) =>
            _issues
                .AsQueryable()
                .Where(x => x.DepartmentNumber == book.DepartmentNumber)
                .OrderByDescending(x => x.IssueDate)
                .ProjectTo<Issue, IssueListlDTO>(_mapper)
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
        /// <returns>Issue details.</returns>
        public async Task<IssueDetailDTO> Borrow(string username, IssueCreateDTO issue)
        {
            var book = await _books.Find(x => x.DepartmentNumber == issue.DepartmentNumber).FirstOrDefaultAsync();
            if (book == null)
            {
                // TODO handle not found                
            }

            // try to mark the book as borrowed
            var result = await _books.UpdateOneAsync(
                x => x.DepartmentNumber == issue.DepartmentNumber && x.IsAvailable,
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
                x => x.DepartmentNumber == issue.DepartmentNumber,
                Builders<Book>.Update
                .AddToSet(x => x.IssueIds, entity.Id)
                .Set(x => x.CurrentIssue, entity));

            return _mapper.Map<IssueDetailDTO>(entity);
        }

        /// <summary>
        /// Return a borrowed book
        /// </summary>
        /// <param name="id">Id of the issue document</param>
        /// <returns></returns>
        public async Task<IssueDetailDTO?> Return(string id)
        {
            var now = DateTime.UtcNow;
            var issue = await _issues.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (issue == null)
            {
                // TODO handle not found
            }

            await _issues.UpdateOneAsync(
                x => x.Id == id,
                Builders<Issue>
                    .Update
                    .Set(x => x.IsReturned, true)
                    .Set(x => x.ReturnDate, now));

            // mark book as available again
            await _books.UpdateOneAsync(
                x => x.DepartmentNumber == issue.DepartmentNumber,
                Builders<Book>.Update
                .Set(x => x.IsAvailable, true)
                .Set(x => x.CurrentIssue.IsReturned, true)
                .Set(x => x.CurrentIssue.ReturnDate, now));

            return _mapper.Map<IssueDetailDTO>(await _issues.Find(x => x.Id == id).FirstAsync());
        }
    }
}
