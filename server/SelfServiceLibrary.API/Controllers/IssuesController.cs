﻿using System;
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
        /// Borrow a book from a library
        /// </summary>
        /// <param name="bookId">Id of the book to be borrowed.</param>
        /// <returns></returns>
        [HttpPost("{bookId}/borrow")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> BorrowAnExistingBookd(Guid bookId) =>
            await _service.Borrow(User.Identity.Name!, bookId, new IssueCreateDTO
            {
                IssueDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(30)
            }) switch
            {
                (false, _) => BadRequest("All books were already issued to someone."),
                (true, IssueDetailDTO detail) => Ok(detail),
                _ => NotFound("Book does not exist."),
            };

        /// <summary>
        /// Return a book for given issue
        /// </summary>
        /// <param name="id">Issue id</param>
        /// <returns></returns>
        [HttpPost("{id}/return")]
        public async Task<ActionResult<IssueDetailDTO>> Return(Guid id) =>
            await _service.Return(id, User.Identity.Name!) switch
            {
                null => NotFound(),
                IssueDetailDTO dto => Ok(dto)
            };
    }
}
