using System.Collections.Generic;
using System.IO;

using SelfServiceLibrary.BL.DTO.Book;

namespace SelfServiceLibrary.BL.Interfaces
{
    public interface ICsvImporter
    {
        /// <summary>
        /// Import book records from CSV
        /// </summary>
        /// <param name="stream">Stream of CSV data, with first line as a header</param>
        /// <returns></returns>
        IAsyncEnumerable<BookImportCsvDTO> ImportBooks(Stream stream);
    }
}
