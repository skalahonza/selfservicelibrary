﻿
using MongoDB.Driver;
using MongoDB.Driver.Linq;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.Extensions;
using SelfServiceLibrary.BL.Filters;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.BL.Responses;
using SelfServiceLibrary.BL.ViewModels;
using SelfServiceLibrary.DAL;
using SelfServiceLibrary.DAL.Entities;
using SelfServiceLibrary.DAL.Enums;
using SelfServiceLibrary.DAL.Queries;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SelfServiceLibrary.BL.Services
{
    public class BookService
    {
        private readonly MongoDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ICsvService _csv;

        public BookService(MongoDbContext dbContext, IMapper mapper, ICsvService csv)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _csv = csv;
        }

        public async Task<PaginatedVM<BookListDTO>> GetAll(int page, int pageSize, ISet<Role> userRoles, IBooksFilter filter, string? publicationType = null)
        {
            var query = _dbContext
                .Books
                .AsQueryable();

            var builder = Builders<Book>.Filter;
            var match = builder.OnlyVisible(userRoles);

            if (!string.IsNullOrEmpty(filter.Departmentnumber))
            {
                //query = query.Where(x => x.DepartmentNumber.Contains(filter.Departmentnumber));
                match &= builder.Regex(x => x.DepartmentNumber, filter.Departmentnumber);
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                //query = query.Where(x => x.Name.Contains(filter.Name));
                match &= builder.Regex(x => x.Name, filter.Name);
            }

            if (!string.IsNullOrEmpty(filter.Author))
            {
                //query = query.Where(x => x.Author.Contains(filter.Author));
                match &= builder.Regex(x => x.Author, filter.Author);
            }

            if (filter.IsAvailable.HasValue)
            {
                query = query.Where(x => x.IsAvailable == filter.IsAvailable.Value);
            }

            if (!string.IsNullOrEmpty(publicationType))
            {
                //query = query.Where(x => x.PublicationType == publicationType);
                match &= builder.Regex(x => x.PublicationType, publicationType);
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                match &= builder.Eq(x => x.Status.Name, filter.Status);
            }

            query = query.Where(_ => match.Inject());

            var count = await query
                .ProjectTo<Book, BookListDTO>(_mapper)
                .CountAsync();

            var data = await query
                .OrderBy(x => x.DepartmentNumber)
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
                .ToDictionary(x => x.Type, x => x.Count);
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
            return types.ToDictionary(x => x.Type, x => x.Count);
        }

        public Task<long> GetTotalCount(ISet<Role> userRoles) =>
            _dbContext.Books.CountDocumentsAsync(Builders<Book>.Filter.OnlyVisible(userRoles));

        public Task<bool> Exists(string departmentNumber) =>
            _dbContext.Books.Find(x => x.DepartmentNumber == departmentNumber).AnyAsync();

        public Task<BookDetailDTO> GetDetail(string departmentNumber) =>
            _dbContext
                .Books
                .AsQueryable()
                .Where(x => x.DepartmentNumber == departmentNumber)
                .ProjectTo<Book, BookDetailDTO>(_mapper)
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

        public async Task<CreateBookResponse> Create(BookAddDTO data)
        {
            try
            {
                await _dbContext.Books.InsertOneAsync(_mapper.Map<Book>(data));
                return new CreateBookResponse(new BookCreated());
            }
            catch (MongoWriteException ex) when (ex.Message.Contains("duplicate key"))
            {
                return new CreateBookResponse(new BookAlreadyExists());
            }
        }

        public async Task Update(string departmentNumber, BookEditDTO data)
        {
            var update = Builders<Book>.Update
                .Set(x => x.Name, data.Name)
                .Set(x => x.Author, data.Author)
                .Set(x => x.SystemNumber, data.SystemNumber)
                .Set(x => x.FelNumber, data.FelNumber)
                .Set(x => x.PublicationType, data.PublicationType)
                .Set(x => x.FormType, data.FormType)
                .Set(x => x.Depended, data.Depended)
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

            await _dbContext
                .Books
                .UpdateOneAsync(x => x.DepartmentNumber == departmentNumber, update);
        }

        public async Task ImportCsv(Stream csv)
        {
            var statuses = (await _dbContext.BookStatuses.AsQueryable()
                .ToListAsync())
                .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

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
                    .SetOnInsert(book => book.IsAvailable, true)
                    // update
                    .Set(book => book.Name, row.Name)
                    .Set(book => book.Author, row.Author)
                    .Set(book => book.CoAuthors, row.CoAuthors)
                    .Set(book => book.PublicationType, row.PublicationType)
                    .Set(book => book.Depended, row.Depended)
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
                    .Set(book => book.NFCIdent, row.NFCIdent)
                    .Set(book => book.QRIdent, row.QRIdent);
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

        public Task ExportCsv(Stream csv, bool leaveOpen = false)
        {
            var books = _dbContext
                .Books
                .AsQueryable()
                .ProjectTo<Book, BookCsvDTO>(_mapper)
                .AsAsyncEnumerable();

            return _csv.ExportBooks(books, csv, leaveOpen);
        }

        public Task DeleteAll() =>
            _dbContext.Books.DeleteManyAsync(Builders<Book>.Filter.Empty);
    }
}
