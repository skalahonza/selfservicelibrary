using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using SelfServiceLibrary.BL.Services;
using SelfServiceLibrary.Web.Filters;
using SelfServiceLibrary.Web.Policies;

namespace SelfServiceLibrary.Web.Pages.Librarian
{
    [Authorize(Policy = LibrarianPolicy.NAME)]
    public class BooksCsvModel : PageModel
    {
        private readonly BookService _bookService;

        public BooksCsvModel(BookService bookService) =>
            _bookService = bookService;

        public async Task<IActionResult> OnGet([FromQuery] BooksFilter? filter)
        {
            var stream = new MemoryStream();
            await _bookService.ExportCsv(stream, filter ?? new BooksFilter(), true);
            stream.Position = 0;
            return File(stream, "text/csv", "books.csv");
        }
    }
}
