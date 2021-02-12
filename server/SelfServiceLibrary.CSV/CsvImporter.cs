using System.Collections.Generic;
using System.Globalization;
using System.IO;

using CsvHelper;
using CsvHelper.Configuration;

using Microsoft.Extensions.Logging;

using SelfServiceLibrary.Service.DTO.Book;
using SelfServiceLibrary.Service.Interfaces;

namespace SelfServiceLibrary.CSV
{
    public class CsvImporter : ICsvImporter
    {
        private readonly ILogger<CsvImporter> _log;

        public CsvImporter(ILogger<CsvImporter> log) => 
            _log = log;

        public async IAsyncEnumerable<BookAddDTO> ImportBooks(Stream stream)
        {
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                TrimOptions = TrimOptions.Trim,
                Delimiter = ";",
                HasHeaderRecord = true,
                BadDataFound = context => _log.LogWarning("Bad CSV data found. {Row}", context.ToString()),
            });
            while (await csv.ReadAsync())
            {
                yield return new BookAddDTO
                {
                    Name = csv.GetField(0),
                    Author = csv.GetField(1),
                    ISBN = csv.GetField(14)
                };
            }
        }
    }
}
