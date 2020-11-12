using System;
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
        /// <param name="issue">Issue data</param>
        /// <returns></returns>
        [HttpPost("borrow")]
        public Task<IssueDetailDTO> Borrow(IssueCreateDTO issue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return a current issue
        /// </summary>
        /// <param name="id">Issue id</param>
        /// <returns></returns>
        [HttpPost("{id}/return")]
        public Task<IssueDetailDTO> Return(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
