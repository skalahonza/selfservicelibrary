using System.Collections.Generic;
using System.IO;

using SelfServiceLibrary.Persistence.Entities;
using SelfServiceLibrary.Service.DTO.Book;

namespace SelfServiceLibrary.Service.Interfaces
{
    public interface ICsvImporter
    {
        /// <summary>
        /// Import book records from CSV
        /// </summary>
        /// <param name="stream">Stream of CSV data, with first line as a header</param>
        /// <returns></returns>
        IAsyncEnumerable<Book> ImportBooks(Stream stream);
    }
}
