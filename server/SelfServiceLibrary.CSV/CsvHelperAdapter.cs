using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CsvHelper;
using CsvHelper.Configuration;

using Microsoft.Extensions.Logging;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.Interfaces;

namespace SelfServiceLibrary.CSV
{
    public class CsvHelperAdapter : ICsvService
    {
        private IMapper _mapper;
        private readonly ILogger<CsvHelperAdapter> _log;

        public CsvHelperAdapter(ILogger<CsvHelperAdapter> log, IMapper mapper)
        {
            _log = log;
            _mapper = mapper;
        }

        public async IAsyncEnumerable<BookCsvDTO> ImportBooks(Stream stream)
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

            await foreach (var book in csv.GetRecordsAsync<BookCSV>().Select(x => _mapper.Map<BookCsvDTO>(x)))
            {
                yield return book;
            }
        }

        public async Task ExportBooks(IAsyncEnumerable<BookCsvDTO> books, Stream outputStream, bool leaveOpen = false)
        {
            using var writer = new StreamWriter(outputStream, leaveOpen: leaveOpen);
            using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                LeaveOpen = leaveOpen,
                TrimOptions = TrimOptions.Trim,
                Delimiter = ";",
                HasHeaderRecord = true
            });
            csv.WriteHeader<BookCSV>();
            csv.NextRecord();
            await foreach (var book in books)
            {
                csv.WriteRecord(_mapper.Map<BookCSV>(book));
                csv.NextRecord();
            }
            await csv.FlushAsync();
        }
    }
}
