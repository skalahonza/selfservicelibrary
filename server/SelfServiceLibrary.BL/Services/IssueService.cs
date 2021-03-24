﻿
using MongoDB.Driver;
using MongoDB.Driver.Linq;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.DTO.Issue;
using SelfServiceLibrary.BL.Extensions;
using SelfServiceLibrary.BL.Interfaces;
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
                .ProjectTo<Issue, IssueListlDTO>(_mapper)
                .ToListAsync();

        public Task<List<IssueListlDTO>> GetAll(string username) =>
            _dbContext
                .Issues
                .AsQueryable()
                .Where(x => x.IssuedTo == username)
                .OrderBy(x => x.IsReturned).ThenBy(x => x.ExpiryDate)
                .ProjectTo<Issue, IssueListlDTO>(_mapper)
                .ToListAsync();

        public Task<List<IssueListlDTO>> GetAll(BookDetailDTO book) =>
            _dbContext
                .Issues
                .AsQueryable()
                .Where(x => x.DepartmentNumber == book.DepartmentNumber)
                .OrderByDescending(x => x.IssueDate)
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
            var book = await _dbContext.Books.Find(x => x.DepartmentNumber == issue.DepartmentNumber).FirstOrDefaultAsync();
            if (book == null)
            {
                // TODO handle not found                
            }

            // try to mark the book as borrowed
            var result = await _dbContext.Books.UpdateOneAsync(
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
            await _dbContext.Issues.InsertOneAsync(entity);

            // link issue to a user
            await _dbContext.Users.UpdateOneAsync(
                x => x.Username == username,
                Builders<User>.Update.AddToSet(x => x.IssueIds, entity.Id),
                new UpdateOptions { IsUpsert = true });

            // link issue to a book
            await _dbContext.Books.UpdateOneAsync(
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
            var issue = await _dbContext.Issues.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (issue == null)
            {
                // TODO handle not found
            }

            await _dbContext.Issues.UpdateOneAsync(
                x => x.Id == id,
                Builders<Issue>
                    .Update
                    .Set(x => x.IsReturned, true)
                    .Set(x => x.ReturnDate, now));

            // mark book as available again
            await _dbContext.Books.UpdateOneAsync(
                x => x.DepartmentNumber == issue.DepartmentNumber,
                Builders<Book>.Update
                .Set(x => x.IsAvailable, true)
                .Set(x => x.CurrentIssue.IsReturned, true)
                .Set(x => x.CurrentIssue.ReturnDate, now));

            return _mapper.Map<IssueDetailDTO>(await _dbContext.Issues.Find(x => x.Id == id).FirstAsync());
        }
    }
}