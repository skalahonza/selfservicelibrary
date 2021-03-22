
using MongoDB.Driver;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.Extensions;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.DAL;
using SelfServiceLibrary.DAL.Entities;

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
        private readonly ICsvImporter _csv;

        public BookService(MongoDbContext dbContext, IMapper mapper, ICsvImporter csv)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _csv = csv;
        }

        public Task<List<BookListDTO>> GetAll(int page, int pageSize) =>
            _dbContext
                .Books
                .AsQueryable()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<Book, BookListDTO>(_mapper)
                .ToListAsync();

        public Task<List<BookListDTO>> GetAll(int page, int pageSize, string publicationType) =>
            _dbContext
                .Books
                .AsQueryable()
                .Where(x => x.PublicationType == publicationType)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<Book, BookListDTO>(_mapper)
                .ToListAsync();

        public async Task<Dictionary<string, int>> GetPublicationTypes()
        {
            var types = await _dbContext
                .Books
                .Aggregate()
                .Group(x => x.PublicationType, x => new
                {
                    Type = x.Key,
                    Count = x.Count()
                }).ToListAsync();
            return types.ToDictionary(x => x.Type, x => x.Count);
        }

        public Task<long> GetTotalCount(bool estimated = true) =>
            estimated
                ? _dbContext.Books.EstimatedDocumentCountAsync()
                : _dbContext.Books.CountDocumentsAsync(Builders<Book>.Filter.Empty);

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
                    return new BookStatus();
                }
                else if (statuses.TryGetValue(intStatus, out var status))
                {
                    // find in existing statuses
                    return status;
                }
                else
                {
                    var newStatus = new BookStatus
                    {
                        Name = intStatus,
                        IsVissible = true,
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

        public Task DeleteAll() =>
            _dbContext.Books.DeleteManyAsync(Builders<Book>.Filter.Empty);
    }
}
