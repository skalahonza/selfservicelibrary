
using MongoDB.Driver;
using MongoDB.Driver.Linq;

using SelfServiceLibrary.BL.DTO.Issue;
using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.BL.Exceptions.Authorization;
using SelfServiceLibrary.BL.Exceptions.Business;
using SelfServiceLibrary.BL.Extensions;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.DAL;
using SelfServiceLibrary.DAL.Entities;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SelfServiceLibrary.BL.Services
{
    public class IssueService : IIssueService
    {
        private readonly MongoDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAuthorizationContext _authorizationContext;

        public IssueService(MongoDbContext dbContext, IMapper mapper, IAuthorizationContext authorizationContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _authorizationContext = authorizationContext;
        }

        public Task<long> GetTotalCount(bool estimated = true) =>
            estimated
                ? _dbContext.Issues.EstimatedDocumentCountAsync()
                : _dbContext.Issues.CountDocumentsAsync(Builders<Issue>.Filter.Empty);

        public Task<List<IssueListlDTO>> GetAll(int page, int pageSize, IEnumerable<(string column, ListSortDirection direction)>? sortings = null) =>
            _dbContext
                .Issues
                .AsQueryable()
                .Sort(sortings)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

        private async Task<IssueDetailDTO> Borrow(IssueCreateDTO details, UserInfoDTO issuedTo, UserInfoDTO issuedBy)
        {
            var book = await _dbContext.Books.Find(x => x.DepartmentNumber == details.DepartmentNumber).FirstOrDefaultAsync();
            if (book == null)
            {
                // handle not found
                throw new EntityNotFoundException<Book>(details.DepartmentNumber);
            }

            // try to mark the book as borrowed
            var result = await _dbContext.Books.UpdateOneAsync(
                x => x.DepartmentNumber == details.DepartmentNumber && x.IsAvailable,
                Builders<Book>.Update.Set(x => x.IsAvailable, false));
            if (result.ModifiedCount == 0)
            {
                // handle book was already taken
                throw new BookIsBorrowedException(details.DepartmentNumber);
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

            return _mapper.Map<IssueDetailDTO>(issue);
        }

        public async Task<IssueDetailDTO> Borrow(IssueCreateDTO details)
        {
            var actor = await _authorizationContext.GetUserInfo();

            if (actor == null || string.IsNullOrEmpty(actor.Username))
            {
                throw new AuthorizationException("Borrow action cannot be performed. Current user's name is empty.");
            }

            if (!await _authorizationContext.CanBorrow())
            {
                throw new AuthorizationException($"User {actor.Username} is not allowed to borrow anything on it's own.");
            }

            return await Borrow(details, actor, actor);
        }

        public async Task<IssueDetailDTO> BorrowTo(IssueCreateDTO details, UserInfoDTO issuedTo)
        {
            var actor = await _authorizationContext.GetUserInfo();

            if (actor == null || string.IsNullOrEmpty(actor.Username))
            {
                throw new AuthorizationException("Borrow action cannot be performed. Current user's name is empty.");
            }

            if (!await _authorizationContext.CanBorrowTo())
            {
                throw new AuthorizationException($"User {actor.Username} is not allowed to borrow a book to someone else.");
            }

            return await Borrow(details, issuedTo, actor);
        }

        private async Task Return(Issue issue, UserInfoDTO returnedBy)
        {
            var now = DateTime.UtcNow;

            var result = await _dbContext.Issues.UpdateOneAsync(
                x => x.Id == issue.Id && !x.IsReturned,
                Builders<Issue>
                    .Update
                    .Set(x => x.IsReturned, true)
                    .Set(x => x.ReturnedBy, _mapper.Map<UserInfo>(returnedBy))
                    .Set(x => x.ReturnDate, now));

            if (result.ModifiedCount == 0)
            {
                // handle book was already returned
                throw new BookAlreadyReturnedException(issue.DepartmentNumber ?? string.Empty);
            }

            // mark book as available again
            await _dbContext.Books.UpdateOneAsync(
                x => x.DepartmentNumber == issue.DepartmentNumber && !x.IsAvailable,
                Builders<Book>.Update
                .Set(x => x.IsAvailable, true)
                .Set(x => x.CurrentIssue!.IsReturned, true)
                .Set(x => x.CurrentIssue!.ReturnDate, now));
        }

        public async Task Return(string id)
        {
            var issue = await _dbContext.Issues.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (issue == null)
            {
                // handle not found
                throw new EntityNotFoundException<Issue>(id);
            }

            var actor = await _authorizationContext.GetUserInfo();

            if (actor == null || string.IsNullOrEmpty(actor.Username))
            {
                throw new AuthorizationException("Return action cannot be performed. Current user's name is empty.");
            }

            // self return
            if (issue.IssuedTo.Username == actor.Username)
            {
                if(!await _authorizationContext.CanBorrow())
                {
                    throw new AuthorizationException($"User {actor.Username} is not allowed to return on it's own.");
                }
            }

            // librarian is returning
            else
            {
                if (!await _authorizationContext.CanBorrowTo())
                {
                    throw new AuthorizationException($"User {actor.Username} is not allowed to return someone else's book.");
                }
            }

            await Return(issue, actor);
        }
    }
}
