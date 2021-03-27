using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.Book;

namespace SelfServiceLibrary.BL.Interfaces
{
    public interface ICsvService
    {
        /// <summary>
        /// Import book records from CSV
        /// </summary>
        /// <param name="stream">Stream of CSV data, with first line as a header</param>
        /// <returns></returns>
        IAsyncEnumerable<BookCsvDTO> ImportBooks(Stream stream);

        /// <summary>
        /// Export book records into CSV
        /// </summary>
        /// <param name="books">Books to export</param>
        /// <param name="outputStream">Stream the CSV will be written to</param>
        /// <param name="leaveOpen">true to leave the System.IO.TextWriter open</param>
        /// <returns></returns>
        Task ExportBooks(IAsyncEnumerable<BookCsvDTO> books, Stream outputStream, bool leaveOpen = false);
    }
}
