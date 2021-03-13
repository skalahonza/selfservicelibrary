﻿using Microsoft.Extensions.Options;

using MongoDB.Driver;

using SelfServiceLibrary.Persistence.Entities;
using SelfServiceLibrary.Persistence.Options;
using SelfServiceLibrary.Service.DTO.Book;
using SelfServiceLibrary.Service.Extensions;
using SelfServiceLibrary.Service.Interfaces;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SelfServiceLibrary.Service.Services
{
    public class BookService
    {
        private readonly IMongoCollection<Book> _books;
        private readonly IMongoCollection<BookStatus> _statuses;
        private readonly IMapper _mapper;
        private readonly ICsvImporter _csv;

        public BookService(IOptions<MongoDbOptions> options, IMongoClient client, IMapper mapper, ICsvImporter csv)
        {
            var database = client.GetDatabase(options.Value.DatabaseName);
            _books = database.GetCollection<Book>(Book.COLLECTION_NAME);
            _statuses = database.GetCollection<BookStatus>(BookStatus.COLLECTION_NAME);
            _mapper = mapper;
            _csv = csv;
        }

        public Task<List<BookListDTO>> GetAll(int page, int pageSize) =>
            _books
                .AsQueryable()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<Book, BookListDTO>(_mapper)
                .ToListAsync();

        public Task<List<BookListDTO>> GetAll(int page, int pageSize, string publicationType) =>
            _books
                .AsQueryable()
                .Where(x => x.PublicationType == publicationType)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<Book, BookListDTO>(_mapper)
                .ToListAsync();

        public async Task<Dictionary<string, int>> GetPublicationTypes()
        {
            var types = await _books
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
                ? _books.EstimatedDocumentCountAsync()
                : _books.CountDocumentsAsync(Builders<Book>.Filter.Empty);

        public Task<BookDetailDTO> GetDetail(string departmentNumber) =>
            _books
                .AsQueryable()
                .Where(x => x.DepartmentNumber == departmentNumber)
                .ProjectTo<Book, BookDetailDTO>(_mapper)
                .FirstOrDefaultAsync();

        public Task<List<BookSearchDTO>> Fulltext(string searchedTerm)
        {
            var query = _books
                .Find(Builders<Book>.Filter.Text(searchedTerm, new TextSearchOptions { CaseSensitive = false, DiacriticSensitive = false }))
                .Project(Builders<Book>.Projection.Expression(x => _mapper.Map<BookSearchDTO>(x)));

            return query.ToListAsync();
        }

        public async Task ImportCsv(Stream csv)
        {
            var statuses = (await _statuses.AsQueryable()
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
                    .Set(book => book.StsUK, row.StsUK);
                    return new UpdateOneModel<Book>(filter, update) { IsUpsert = true };
                });

            // bulk insert books
            await foreach (var batch in writes.Batchify(1000))
            {
                await _books.BulkWriteAsync(batch);
            }

            // insert new statuses
            await _statuses.BulkWriteAsync(newStatuses.Select(x =>
            {
                var filter = Builders<BookStatus>.Filter.Where(status => status.Name == x.Name);
                return new ReplaceOneModel<BookStatus>(filter, x) { IsUpsert = true };
            }));
        }

        public Task DeleteAll() =>
            _books.DeleteManyAsync(Builders<Book>.Filter.Empty);
    }
}
