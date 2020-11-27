using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using SelfServiceLibrary.API.Services;
using SelfServiceLibrary.BL.DTO.Issue;

namespace SelfServiceLibrary.API.Controllers
{
    public class IssuesController : BaseController
    {
        private readonly IssueService _service;

        public IssuesController(IssueService service) =>
            _service = service;

        /// <summary>
        /// Get all issues
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Task<List<IssueListlDTO>> GetAll() =>
            _service.GetAll();

        /// <summary>
        /// Borrow a book from a library
        /// </summary>
        /// <param name="bookId">Id of the book to be borrowed.</param>
        /// <param name="issue">Issue details.</param>
        /// <returns></returns>
        [HttpPost("{bookId}/borrow")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> BorrowAnExistingBookd(Guid bookId, IssueCreateDTO issue) =>
            await _service.Borrow(User.Identity.Name ?? string.Empty, bookId, issue) switch
            {
                (false, _) => BadRequest("All books were already issued to someone."),
                (true, IssueDetailDTO detail) => Ok(detail),
                _ => NotFound("Book does not exist."),
            };

        /// <summary>
        /// Return a current issue
        /// </summary>
        /// <param name="id">Issue id</param>
        /// <returns></returns>
        [HttpPost("{id}/return")]
        public async Task<ActionResult<IssueDetailDTO>> Return(Guid id) =>
            await _service.Return(id) switch
            {
                null => NotFound(),
                IssueDetailDTO dto => Ok(dto)
            };
    }
}
