
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using SelfServiceLibrary.Service.DTO.Issue;
using SelfServiceLibrary.Service.Services;

namespace SelfServiceLibrary.API.Controllers
{
    public class BooksController : BaseController
    {
        private readonly IssueService _service;

        public BooksController(IssueService service) =>
            _service = service;

        /// <summary>
        /// Borrow a book from a library
        /// </summary>
        /// <returns></returns>
        [HttpPost("borrow")]
        public async Task<ActionResult<IssueDetailDTO>> Borrow([FromBody] NfcIssueCreateDTO[] issues)
        {
            if (issues.Length == 0) return BadRequest();
            if (string.IsNullOrEmpty(User.Identity?.Name)) return Unauthorized();

            // await _service.Borrow(User.Identity.Name, issues);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return a previously borrowed book
        /// </summary>
        /// <returns></returns>
        [HttpPost("return")]
        public async Task<IActionResult> Return([FromBody] NfcIssueReturnDTO[] issues)
        {
            if (issues.Length == 0) return BadRequest();
            throw new NotImplementedException();
        }
    }
}
