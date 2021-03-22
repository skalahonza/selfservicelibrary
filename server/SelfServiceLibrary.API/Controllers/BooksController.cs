using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<GetBookResponse>> Get(string serNumNFC)
        {
            var book = await _service.GetByNFC(serNumNFC);
            return book is not null
                ? Ok(new GetBookResponse($"/books/{book.DepartmentNumber}"))
                : NotFound();
        }
    }
}
