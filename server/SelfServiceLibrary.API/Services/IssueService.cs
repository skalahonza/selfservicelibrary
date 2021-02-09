using AutoMapper;

using Microsoft.Extensions.Options;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

using SelfServiceLibrary.API.Options;
using SelfServiceLibrary.BL.DTO.Issue;
using SelfServiceLibrary.BL.Entities;

using System;
using System.Collections.Generic;
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

        public Task<List<IssueListlDTO>> GetAll(string username) =>
            _issues.Find(Builders<Issue>.Filter.Where(x => x.IssuedTo == username))
                .Project(Builders<Issue>.Projection.Expression(x => _mapper.Map<IssueListlDTO>(x)))
                .ToListAsync();

        /// <summary>
        /// Borrow a book if available
        /// </summary>
        /// <param name="issue">Issue details.</param>
        /// <param name="username">To whom will the book be issued.</param>
        /// <param name="bookId">Id of the book to borrow.</param>
        /// <returns>Null if book not found, true if borrowed, false if out of capacity</returns>
        public async Task<(bool?, IssueDetailDTO?)> Borrow(string username, Guid bookId, IssueCreateDTO issue)
        {
            var book = await _books.Find(x => x.Id == bookId).FirstOrDefaultAsync();
            if (book == null) return (null, null);
            var update = Builders<Book>.Update.Inc(x => x.Issued, 1);
            var result = await _books.UpdateOneAsync(x => x.Id == bookId && x.Issued < book.Quantity, update);
            if (result.ModifiedCount == 0) return (false, null);

            var entity = new Issue { Id = Guid.NewGuid(), IssuedTo = username };
            entity = _mapper.Map(issue, entity);
            entity = _mapper.Map(book, entity);
            await _issues.InsertOneAsync(entity);

            return (true, _mapper.Map<IssueDetailDTO>(entity));
        }

        public async Task<IssueDetailDTO?> Return(Guid id, string username)
        {
            var update = Builders<Issue>.Update
                .Set(x => x.ReturnDate, DateTime.UtcNow)
                .Set(x => x.IsReturned, true);
            var result = await _issues.UpdateOneAsync(x => x.Id == id
                    && x.IssuedTo == username
                    && !x.IsReturned,
                update);
            if (result.ModifiedCount == 0) return null;
            var issue = await _issues.Find(x => x.Id == id).FirstOrDefaultAsync();
            await _books.UpdateOneAsync(x => x.Id == issue.BookId, Builders<Book>.Update.Inc(x => x.Issued, -1));
            return _mapper.Map<IssueDetailDTO>(issue);
        }
    }
}
