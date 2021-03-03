
using System;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using SelfServiceLibrary.Service.Interfaces;
using SelfServiceLibrary.Service.Services;

namespace SelfServiceLibrary.API.Controllers
{
    public class BooksController : BaseController
    {
        private readonly IssueService _service;
        private readonly ICardAuthenticator _authenticator;

        public BooksController(IssueService service, ICardAuthenticator authenticator)
        {
            _service = service;
            _authenticator = authenticator;
        }

        /// <summary>
        /// Borrow a book from a library
        /// </summary>
        /// <param name="departmentNumber">Department number of the book to be borrowed.</param>
        /// <returns></returns>
        [HttpPost("{departmentNumber}/borrow")]
        public async Task<IActionResult> Borrow(string departmentNumber)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return a previously borrowed book
        /// </summary>
        /// <param name="departmentNumber">Department number of the book to be returned.</param>
        /// <returns></returns>
        [HttpPost("{departmentNumber}/return")]
        public async Task<IActionResult> Return(string departmentNumber)
        {
            throw new NotImplementedException();
        }
    }
}
