using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using CsvHelper;
using CsvHelper.Configuration;

using Microsoft.Extensions.Logging;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.Interfaces;

namespace SelfServiceLibrary.CSV
{
    public class CsvImporter : ICsvImporter
    {
        private readonly ILogger<CsvImporter> _log;

        public CsvImporter(ILogger<CsvImporter> log) =>
            _log = log;

        private static int? TryParseInt(string? value)
        {
            if (int.TryParse(value, out var number))
                return number;
            return null;
        }

        private static double? TryParseDouble(string? value)
        {
            if (double.TryParse(value, out var number))
                return number;
            return null;
        }

        public async IAsyncEnumerable<BookImportCsvDTO> ImportBooks(Stream stream)
        {
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                TrimOptions = TrimOptions.Trim,
                Delimiter = ";",
                HasHeaderRecord = true,
                BadDataFound = context => _log.LogWarning(
                    "Bad CSV data found. {RowNumber} {RowData}",
                    context.Parser.Row,
                    context.Parser.RawRecord),
                MissingFieldFound = (headers, index, context) => _log.LogWarning(
                    "Missing field in row {RowNumber} at: {Index}",
                    context.Parser.Row,
                    index)
            });

            // process header
            await csv.ReadAsync();
            csv.ReadHeader();

            while (await csv.ReadAsync())
            {
                yield return new BookImportCsvDTO
                {
                    Name = csv.GetField(0),
                    Author = csv.GetField(1),
                    CoAuthors = csv.GetField(2).Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList(),
                    PublicationType = csv.GetField(3),
                    Depended = csv.GetField(4),
                    SystemNumber = csv.GetField(5),
                    FelNumber = csv.GetField(6),
                    DepartmentNumber = csv.GetField(7),
                    BarCode = csv.GetField(8),
                    Pages = TryParseInt(csv.GetField(9)),
                    Publication = TryParseInt(csv.GetField(10).Split('.').FirstOrDefault()),
                    YearOfPublication = TryParseInt(csv.GetField(11)),
                    Publisher = csv.GetField(12),
                    CountryOfPublication = csv.GetField(13),
                    ISBNorISSN = csv.GetField(14),
                    MagazineNumber = csv.GetField(15),
                    MagazineYear = TryParseInt(csv.GetField(16)),
                    Conference = csv.GetField(17),
                    Price = TryParseDouble(csv.GetField(18)),
                    Keywords = csv.GetField(19).Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList(),
                    Note = csv.GetField(20),
                    FormType = csv.GetField(25),
                    IntStatus = csv.GetField(26),
                    StsLocal = !string.IsNullOrEmpty(csv.GetField(30)),
                    StsUK = !string.IsNullOrEmpty(csv.GetField(31)),
                    NFCIdent = csv.GetField("NfcCode"),
                    QRIdent = csv.GetField("QRCode"),
                };
            }
        }
    }
}
