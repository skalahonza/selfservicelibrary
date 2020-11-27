using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SelfServiceLibrary.API.Services;
using SelfServiceLibrary.BL.DTO.Book;

namespace SelfServiceLibrary.API.Controllers
{
    public class BooksController : BaseController
    {
        private readonly BookService _bookService;

        public BooksController(BookService bookService) =>
            _bookService = bookService;

        /// <summary>
        /// Get list of all books from the library
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public Task<List<BookListDTO>> ListBooks() =>
            _bookService.GetAll();

        /// <summary>
        /// Get book detail
        /// </summary>
        /// <param name="id">Book id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<BookDetailDTO>> BookDetail(Guid id) =>
            await _bookService.GetDetail(id) switch
            {
                null => NotFound(),
                BookDetailDTO x => Ok(x)
            };

        /// <summary>
        /// Add a new book to the library
        /// </summary>
        /// <param name="book">Book data</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public Task<BookDetailDTO> AddBook(BookAddDTO book) =>
            _bookService.Add(book);

        /// <summary>
        /// Edit existing book.
        /// </summary>
        /// <param name="id">Book id</param>
        /// <param name="book">Book data to be edited</param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<BookDetailDTO>> PatchBook(Guid id, BookEditDTO book) =>
            await _bookService.Path(id, book) switch
            {
                null => NotFound(),
                BookDetailDTO x => Ok(x)
            };

        /// <summary>
        /// Deletes existing book
        /// </summary>
        /// <param name="id">Book id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> BookDelete(Guid id) =>
            await _bookService.Delete(id)
                ? Ok() as IActionResult
                : NotFound();
    }
}
