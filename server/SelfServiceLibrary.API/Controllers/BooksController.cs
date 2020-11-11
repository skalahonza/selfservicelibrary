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
        private readonly BookService _service;

        public BooksController(BookService service) =>
            _service = service;

        /// <summary>
        /// Get list of all books from the library
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public Task<List<BookListDTO>> ListBooks() =>
            _service.GetAll();

        /// <summary>
        /// Get book detail
        /// </summary>
        /// <param name="id">Book id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public Task<ActionResult<BookDetailDTO>> BookDetail(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a new book to the library
        /// </summary>
        /// <param name="book">Book data</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public Task<ActionResult<BookDetailDTO>> AddBook(BookAddDTO book)
        {
            throw new NotImplementedException();
        }

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
        public Task<ActionResult<BookDetailDTO>> EditBook(string id, BookEditDTO book)
        {
            throw new NotImplementedException();
        }
    }
}
