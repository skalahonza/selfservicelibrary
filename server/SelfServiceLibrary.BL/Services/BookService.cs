
using MongoDB.Driver;
using MongoDB.Driver.Linq;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.BL.Exceptions;
using SelfServiceLibrary.BL.Extensions;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.BL.ViewModels;
using SelfServiceLibrary.DAL;
using SelfServiceLibrary.DAL.Entities;
using SelfServiceLibrary.DAL.Enums;
using SelfServiceLibrary.DAL.Filters;
using SelfServiceLibrary.DAL.Queries;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SelfServiceLibrary.BL.Services
{
    public class BookService : IBookService
    {
        public const int MAX_PAGESIZE = 250;

        private readonly MongoDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ICsvService _csv;
        private readonly IAuthorizationContext _authorizationContext;

        public BookService(MongoDbContext dbContext, IMapper mapper, ICsvService csv, IAuthorizationContext authorizationContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _csv = csv;
            _authorizationContext = authorizationContext;
        }

        public async Task<PaginatedVM<BookListDTO>> GetAll(int page, int pageSize, IBooksFilter filter, IEnumerable<(string column, ListSortDirection direction)>? sortings = null)
        {
            pageSize = Math.Min(MAX_PAGESIZE, pageSize);

            var query = _dbContext
                .Books
                .AsQueryable()
                .Filter(filter);

            var count = await query
                .AsMongoDbQueryable()
                .CountAsync();

            // sorting
            query = query.Sort(sortings);

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<Book, BookListDTO>(_mapper)
                .ToListAsync();

            return new PaginatedVM<BookListDTO>(count, data);
        }

        public async Task<Dictionary<string, int>> GetFormTypes()
        {
            var types = await _dbContext
                .Books
                .Aggregate()
                .Group(x => x.FormType, x => new
                {
                    Type = x.Key,
                    Count = x.Count()
                }).ToListAsync();
            return types
                .Where(x => !string.IsNullOrEmpty(x.Type))
                .ToDictionary(x => x.Type!, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetPublicationTypes(IBooksFilter filter)
        {
            var types = await _dbContext
                   .Books
                   .AsQueryable()
                   .Filter(filter)
                   .GroupBy(x => x.PublicationType, (key, g) => new
                   {
                       Type = key,
                       Count = g.Count()
                   })
                   .AsMongoDbQueryable()
                   .ToListAsync();
            return types
                .Where(x => !string.IsNullOrEmpty(x.Type))
                .ToDictionary(x => x.Type!, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetPublicationTypes(ISet<Role> userRoles)
        {
            var types = await _dbContext
                .Books
                .Aggregate()
                .Match(Builders<Book>.Filter.OnlyVisible(userRoles))
                .Group(x => x.PublicationType, x => new
                {
                    Type = x.Key,
                    Count = x.Count()
                }).ToListAsync();

            return types
                .Where(x => !string.IsNullOrEmpty(x.Type))
                .ToDictionary(x => x.Type!, x => x.Count);
        }

        public Task<int> GetTotalCount(IBooksFilter filter) =>
            _dbContext
                .Books
                .AsQueryable()
                .Filter(filter)
                .AsMongoDbQueryable()
                .CountAsync();

        public Task<bool> Exists(string departmentNumber) =>
            _dbContext.Books.Find(x => x.DepartmentNumber == departmentNumber).AnyAsync();

        public Task<BookDetailDTO?> GetDetail(string departmentNumber) =>
             _dbContext
                .Books
                .AsQueryable()
                .Where(x => x.DepartmentNumber == departmentNumber)
                .ProjectTo<Book, BookDetailDTO?>(_mapper)
                .FirstOrDefaultAsync();

        public Task<BookListDTO> GetByNFC(string serNumNFC) =>
            _dbContext
                .Books
                .AsQueryable()
                .Where(x => x.NFCIdent == serNumNFC)
                .ProjectTo<Book, BookListDTO>(_mapper)
                .FirstOrDefaultAsync();

        public async Task<List<BookSearchDTO>> Fulltext(string searchedTerm)
        {
            // https://stackoverflow.com/questions/32194379/mongodb-text-search-with-sorting-in-c-sharp/32194762
            var F = Builders<Book>.Filter.Text(searchedTerm, new TextSearchOptions { CaseSensitive = false, DiacriticSensitive = false });
            var P = Builders<Book>.Projection.MetaTextScore("TextMatchScore");
            var S = Builders<Book>.Sort.MetaTextScore("TextMatchScore");

            var query = _dbContext
                .Books
                .Find(F)
                .Project<Book>(P)
                .Limit(100)
                .Sort(S);

            var results = await query.ToListAsync();

            return _mapper.Map<List<BookSearchDTO>>(results);
        }

        public Task<bool> HasReviewed(string departmentNumber, string username) =>
            _dbContext
                .Books
                .Find(x => x.DepartmentNumber == departmentNumber && x.Reviews.Any(x => x.Username == username))
                .AnyAsync();

        public Task<bool> HasRead(string departmentNumber, string username) =>
            _dbContext
                .Issues
                .Find(x =>
                        x.DepartmentNumber == departmentNumber &&
                        x.IssuedTo.Username == username &&
                        x.IsReturned
                )
                .AnyAsync();

        public async Task<List<UserInfoDTO>> GetWatchdogs(string departmentNumber)
        {
            var watchdogs = (await _dbContext
                .Books
                .AsQueryable()
                .Where(x => x.DepartmentNumber == departmentNumber)
                .Select(x => x.Watchdogs)
                .AsMongoDbQueryable()
                .FirstOrDefaultAsync())
                ?? new List<UserInfo>();

            return _mapper.Map<List<UserInfoDTO>>(watchdogs);
        }

        public async Task RegisterWatchdog(string departmentNumber)
        {
            var actor = await _authorizationContext.GetUserInfo();

            if (actor == null || string.IsNullOrEmpty(actor.Username))
            {
                throw new AuthorizationException("Cannot register a Watch Dog. Current user's name is empty.");
            }

            // add Watchdog if not present already
            var update = Builders<Book>
                .Update
                .Push(x => x.Watchdogs, _mapper.Map<UserInfo>(actor));

            await _dbContext.Books.UpdateOneAsync(x =>
                x.DepartmentNumber == departmentNumber &&
                !x.Watchdogs.Any(x => x.Username == actor.Username),
                update);
        }

        public Task<bool> HasWatchdog(string departmentNumber, string username) =>
             _dbContext
                    .Books
                    .Find(x => x.DepartmentNumber == departmentNumber && x.Watchdogs.Any(x => x.Username == username))
                    .AnyAsync();

        public Task ClearWatchdogs(string departmentNumber)
        {
            var update = Builders<Book>
                .Update
                .Set(x => x.Watchdogs, new List<UserInfo>());

            return _dbContext.Books.UpdateOneAsync(x => x.DepartmentNumber == departmentNumber, update);
        }

        public async Task Create(BookAddDTO data)
        {
            if (!await _authorizationContext.CanManageContent())
            {
                throw new AuthorizationException("Insufficient permissions for adding a book.");
            }

            if (string.IsNullOrEmpty(data.DepartmentNumber))
                throw new ArgumentException("DepartmentNumber cannot be null or empty.");

            try
            {
                await _dbContext.Books.InsertOneAsync(_mapper.Map<Book>(data));
            }
            catch (MongoWriteException ex) when (ex.Message.Contains("duplicate key"))
            {
                throw new BookAlreadyExistsException(data.DepartmentNumber);
            }
        }

        public async Task Update(string departmentNumber, BookEditDTO data)
        {
            if (!await _authorizationContext.CanManageContent())
            {
                throw new AuthorizationException("Insufficient permissions for updating a book.");
            }

            var bookUpdate = Builders<Book>.Update
                .Set(x => x.Name, data.Name)
                .Set(x => x.Author, data.Author)
                .Set(x => x.CoAuthors, data.CoAuthors)
                .Set(x => x.SystemNumber, data.SystemNumber)
                .Set(x => x.FelNumber, data.FelNumber)
                .Set(x => x.PublicationType, data.PublicationType)
                .Set(x => x.FormType, data.FormType)
                .Set(x => x.Keywords, data.Keywords)
                .Set(x => x.Storage, data.Storage)
                .Set(x => x.Conference, data.Conference)
                .Set(x => x.Note, data.Note)
                .Set(x => x.CountryOfPublication, data.CountryOfPublication)
                .Set(x => x.Publication, data.Publication)
                .Set(x => x.Publisher, data.Publisher)
                .Set(x => x.YearOfPublication, data.YearOfPublication)
                .Set(x => x.Pages, data.Pages)
                .Set(x => x.Price, data.Price)
                .Set(x => x.MagazineNumber, data.MagazineNumber)
                .Set(x => x.MagazineYear, data.MagazineYear)
                .Set(x => x.ISBNorISSN, data.ISBNorISSN)
                .Set(x => x.NFCIdent, data.NFCIdent)
                .Set(x => x.BarCode, data.BarCode)
                .Set(x => x.StsLocal, data.StsLocal)
                .Set(x => x.StsUK, data.StsUK)
                .Set(x => x.Status, await _dbContext.BookStatuses.Find(x => x.Name == data.StatusName).FirstOrDefaultAsync());

            var issueUpdate = Builders<Issue>.Update
                .Set(x => x.BookName, data.Name);

            await _dbContext
                .Books
                .UpdateOneAsync(x => x.DepartmentNumber == departmentNumber, bookUpdate);

            await _dbContext
                .Issues
                .UpdateManyAsync(x => x.DepartmentNumber == departmentNumber, issueUpdate);
        }

        public async Task ImportCsv(Stream csv)
        {
            if (!await _authorizationContext.CanManageContent())
            {
                throw new AuthorizationException("Insufficient permissions for importing CSV with books.");
            }

            var enteredBy = await _authorizationContext.GetUserInfo();

            if(enteredBy == null)
            {
                throw new AuthorizationException("Current user is name is empty.");
            }

            var statuses = (await _dbContext.BookStatuses.AsQueryable()
                .ToListAsync())
                .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

            var enteredByEntity = _mapper.Map<UserInfo>(enteredBy);

            var newStatuses = new List<BookStatus> { new BookStatus() };

            BookStatus MapStatus(string? intStatus)
            {
                if (string.IsNullOrEmpty(intStatus))
                {
                    // default status
                    return new BookStatus();
                }
                else if (statuses.TryGetValue(intStatus, out var status))
                {
                    // found in existing statuses
                    return status;
                }
                else
                {
                    // not found in existing statuses
                    var newStatus = new BookStatus
                    {
                        Name = intStatus,
                        IsVisible = true,
                        CanBeBorrowed = false
                    };
                    newStatuses.Add(newStatus);
                    return newStatus;
                }
            }

            var writes = _csv.ImportBooks(csv)
                .Select(row =>
                {
                    var filter = Builders<Book>.Filter.Where(book => book.DepartmentNumber == row.DepartmentNumber);
                    var update = Builders<Book>.Update
                    // first insert only
                    .SetOnInsert(book => book.Entered, DateTime.UtcNow)
                    .SetOnInsert(book => book.EnteredBy, enteredByEntity)
                    .SetOnInsert(book => book.IsAvailable, true)
                    .SetOnInsert(book => book.Reviews, new List<BookReview>())
                    .SetOnInsert(book => book.Watchdogs, new List<UserInfo>())
                    // update
                    .Set(book => book.Name, row.Name)
                    .Set(book => book.Author, row.Author)
                    .Set(book => book.CoAuthors, row.CoAuthors)
                    .Set(book => book.PublicationType, row.PublicationType)
                    .Set(book => book.Storage, row.Storage)
                    .Set(book => book.SystemNumber, row.SystemNumber)
                    .Set(book => book.FelNumber, row.FelNumber)
                    .Set(book => book.BarCode, row.BarCode)
                    .Set(book => book.Pages, row.Pages)
                    .Set(book => book.Publication, row.Publication)
                    .Set(book => book.YearOfPublication, row.YearOfPublication)
                    .Set(book => book.Publisher, row.Publisher)
                    .Set(book => book.CountryOfPublication, row.CountryOfPublication)
                    .Set(book => book.ISBNorISSN, row.ISBNorISSN)
                    .Set(book => book.MagazineNumber, row.MagazineNumber)
                    .Set(book => book.MagazineYear, row.MagazineYear)
                    .Set(book => book.Conference, row.Conference)
                    .Set(book => book.Price, row.Price)
                    .Set(book => book.Keywords, row.Keywords)
                    .Set(book => book.Note, row.Note)
                    .Set(book => book.Status, MapStatus(row.IntStatus))
                    .Set(book => book.FormType, row.FormType)
                    .Set(book => book.StsLocal, row.StsLocal)
                    .Set(book => book.StsUK, row.StsUK)
                    .Set(book => book.NFCIdent, row.NFCIdent);
                    return new UpdateOneModel<Book>(filter, update) { IsUpsert = true };
                });

            // bulk insert books
            await foreach (var batch in writes.Batchify(1000))
            {
                await _dbContext.Books.BulkWriteAsync(batch);
            }

            // insert new statuses
            await _dbContext.BookStatuses.BulkWriteAsync(newStatuses.Select(x =>
            {
                var filter = Builders<BookStatus>.Filter.Where(status => status.Name == x.Name);
                return new ReplaceOneModel<BookStatus>(filter, x) { IsUpsert = true };
            }));
        }

        public async Task ExportCsv(Stream csv, IBooksFilter filter, bool leaveOpen = false)
        {
            var books = _dbContext
                .Books
                .AsQueryable()
                .Filter(filter)
                .ProjectTo<Book, BookCsvDTO>(_mapper)
                .AsAsyncEnumerable();

            await _csv.ExportBooks(books, csv, leaveOpen);
        }

        public async Task Delete(string departmentNumber)
        {
            if (!await _authorizationContext.CanManageContent())
            {
                throw new AuthorizationException("Insufficient permissions for deleting a book.");
            }

            var result = await _dbContext
                .Books
                .DeleteOneAsync(x => x.DepartmentNumber == departmentNumber && x.IsAvailable);

            if (result.DeletedCount == 0)
            {
                if (!await Exists(departmentNumber))
                {
                    throw new EntityNotFoundException<Book>(departmentNumber);
                }
                else
                {
                    throw new BookIsBorrowedException(departmentNumber);
                }
            }
        }

        public async Task DeleteAll()
        {
            if (!await _authorizationContext.CanManageContent())
            {
                throw new AuthorizationException("Insufficient permissions for deleting all books.");
            }

            await _dbContext.Books.DeleteManyAsync(Builders<Book>.Filter.Where(x => x.IsAvailable));
        }

        public async Task AddOrUpdateReview(BookReviewDTO review)
        {
            // create review if not existed
            var update = Builders<Book>
                .Update
                .Push(x => x.Reviews, _mapper.Map<BookReview>(review));

            await _dbContext.Books.UpdateOneAsync(x =>
                x.DepartmentNumber == review.DepartmentNumber && !x.Reviews.Any(x => x.Username == review.Username),
                update);

            // update existing review
            var filter = Builders<Book>.Filter.Eq(x => x.DepartmentNumber, review.DepartmentNumber)
                & Builders<Book>.Filter.ElemMatch(x => x.Reviews, x => x.Username == review.Username);

            update = Builders<Book>
                .Update
                // [-1] is translated to .$ which means an array item that was found with the filter elem match
                .Set(x => x.Reviews[-1].Value, review.Value);

            await _dbContext.Books.UpdateOneAsync(filter, update);
        }
    }
}
