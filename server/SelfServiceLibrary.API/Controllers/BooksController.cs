
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SelfServiceLibrary.Service.DTO.Issue;
using SelfServiceLibrary.Service.Services;

namespace SelfServiceLibrary.API.Controllers
{
    public class BooksController : BaseController
    {
        private readonly BookService _service;

        public BooksController(BookService service) =>
            _service = service;

        public record GetBookResponse(string Url);

        /// <summary>
        /// Find book by 
        /// </summary>
        /// <param name="serNumNFC"></param>
        /// <returns></returns>
        [HttpGet("{serNumNFC}")]
        [AllowAnonymous]
        public async Task<ActionResult<GetBookResponse>> Get(string serNumNFC)
        {
            var book = await _service.GetByNFC(serNumNFC);
            return book is not null
                ? Ok(new GetBookResponse($"/books/{book.DepartmentNumber}"))
                : NotFound();
        }

        ///// <summary>
        ///// Borrow a book from a library
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost("borrow")]
        //public async Task<ActionResult<IssueDetailDTO>> Borrow([FromBody] NfcIssueCreateDTO[] issues)
        //{
        //    if (issues.Length == 0) return BadRequest();
        //    if (string.IsNullOrEmpty(User.Identity?.Name)) return Unauthorized();

        //    // await _service.Borrow(User.Identity.Name, issues);
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Return a previously borrowed book
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost("return")]
        //public async Task<IActionResult> Return([FromBody] NfcIssueReturnDTO[] issues)
        //{
        //    if (issues.Length == 0) return BadRequest();
        //    throw new NotImplementedException();
        //}
    }
}
