
using MongoDB.Driver;
using MongoDB.Driver.Linq;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.DTO.Issue;
using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.BL.Extensions;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.BL.Responses;
using SelfServiceLibrary.DAL;
using SelfServiceLibrary.DAL.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SelfServiceLibrary.BL.Services
{
    public class IssueService
    {
        private readonly MongoDbContext _dbContext;
        private readonly IMapper _mapper;

        public IssueService(MongoDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public Task<long> GetTotalCount(bool estimated = true) =>
            estimated
                ? _dbContext.Issues.EstimatedDocumentCountAsync()
                : _dbContext.Issues.CountDocumentsAsync(Builders<Issue>.Filter.Empty);

        public Task<List<IssueListlDTO>> GetAll(int page, int pageSize) =>
            _dbContext
                .Issues
                .AsQueryable()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(x => x.IsReturned)
                .ThenBy(x => x.ExpiryDate)
                .ProjectTo<Issue, IssueListlDTO>(_mapper)
                .ToListAsync();

        public Task<List<IssueListlDTO>> GetAll(string username) =>
            _dbContext
                .Issues
                .AsQueryable()
                .Where(x => x.IssuedTo.Username == username)
                .OrderBy(x => x.IsReturned)
                .ThenBy(x => x.ExpiryDate)
                .ProjectTo<Issue, IssueListlDTO>(_mapper)
                .ToListAsync();

        public Task<List<IssueListlDTO>> GetBookIssues(string departmentNumber) =>
            _dbContext
                .Issues
                .AsQueryable()
                .Where(x => x.DepartmentNumber == departmentNumber)
                .OrderByDescending(x => x.IssueDate)
                .ProjectTo<Issue, IssueListlDTO>(_mapper)
                .ToListAsync();

        /// <summary>
        /// Borrow a book if available
        /// </summary>
        /// <param name="issuedTo">To whom will the book be issued.</param>
        /// <param name="details">Issue details.</param>
        /// <param name="issuedBy">By whom was the book issued, leave empty in case of self-service borrowing.</param>
        /// <returns>Issue details.</returns>
        public async Task<BorrowResponse> Borrow(UserInfoDTO issuedTo, IssueCreateDTO details, UserInfoDTO? issuedBy = null)
        {
            issuedBy ??= issuedTo;
            var book = await _dbContext.Books.Find(x => x.DepartmentNumber == details.DepartmentNumber).FirstOrDefaultAsync();
            if (book == null)
            {
                // handle not found
                return new BorrowResponse(new BookNotFound());
            }

            // try to mark the book as borrowed
            var result = await _dbContext.Books.UpdateOneAsync(
                x => x.DepartmentNumber == details.DepartmentNumber && x.IsAvailable,
                Builders<Book>.Update.Set(x => x.IsAvailable, false));
            if (result.ModifiedCount == 0)
            {
                // handle book was already taken
                return new BorrowResponse(new BookIsBorrowed());
            }

            // create issue document
            var issue = new Issue
            {
                Id = Guid.NewGuid().ToString(),
                IssueDate = DateTime.UtcNow,
                IssuedTo = _mapper.Map<UserInfo>(issuedTo),
                IssuedBy = _mapper.Map<UserInfo>(issuedBy)
            };
            issue = _mapper.Map(details, issue);
            issue = _mapper.Map(book, issue);
            await _dbContext.Issues.InsertOneAsync(issue);

            // link issue to a book
            await _dbContext.Books.UpdateOneAsync(
                x => x.DepartmentNumber == details.DepartmentNumber,
                Builders<Book>.Update
                .Set(x => x.CurrentIssue, issue));

            return new BorrowResponse(_mapper.Map<IssueDetailDTO>(issue));
        }

        /// <summary>
        /// Return a borrowed book
        /// </summary>
        /// <param name="id">Id of the issue document</param>
        /// <param name="returnedBy">By whom was the book returned</param>
        /// <returns></returns>
        public async Task<ReturnResponse> Return(string id, UserInfoDTO returnedBy)
        {
            var now = DateTime.UtcNow;
            var issue = await _dbContext.Issues.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (issue == null)
            {
                // handle not found
                return new ReturnResponse(new IssueNotFound());
            }

            var result = await _dbContext.Issues.UpdateOneAsync(
                x => x.Id == id && !x.IsReturned,
                Builders<Issue>
                    .Update
                    .Set(x => x.IsReturned, true)
                    .Set(x => x.ReturnedBy, _mapper.Map<UserInfo>(returnedBy))
                    .Set(x => x.ReturnDate, now));

            if (result.ModifiedCount == 0)
            {
                // handle book was already returned
                return new ReturnResponse(new BookAlreadyReturned());
            }

            // mark book as available again
            await _dbContext.Books.UpdateOneAsync(
                x => x.DepartmentNumber == issue.DepartmentNumber && !x.IsAvailable,
                Builders<Book>.Update
                .Set(x => x.IsAvailable, true)
                .Set(x => x.CurrentIssue.IsReturned, true)
                .Set(x => x.CurrentIssue.ReturnDate, now));

            return new ReturnResponse(new BookReturned());
        }
    }
}
