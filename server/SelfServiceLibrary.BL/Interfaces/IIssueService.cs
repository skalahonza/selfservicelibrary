using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.Issue;
using SelfServiceLibrary.BL.DTO.User;

namespace SelfServiceLibrary.BL.Interfaces
{
    public interface IIssueService
    {
        /// <summary>
        /// Borrow an existing book
        /// </summary>
        /// <param name="issuedTo"></param>
        /// <param name="details"></param>
        /// <param name="issuedBy"></param>
        /// <exception cref="Exceptions.Business.EntityNotFoundException{Book}">Thrown when the book is not found.</exception>
        /// <exception cref="Exceptions.Business.BookIsBorrowedException">Thrown when the book is currently borrowed.</exception>
        /// <returns></returns>
        Task<IssueDetailDTO> Borrow(UserInfoDTO issuedTo, IssueCreateDTO details, UserInfoDTO? issuedBy = null);
        Task<List<IssueListlDTO>> GetAll(int page, int pageSize, IEnumerable<(string column, ListSortDirection direction)>? sortings = null);
        Task<List<IssueListlDTO>> GetAll(string username);
        Task<List<IssueListlDTO>> GetBookIssues(string departmentNumber);
        Task<long> GetTotalCount(bool estimated = true);
        /// <summary>
        /// Return a previously borrowed book.
        /// </summary>
        /// <param name="id">Issue document id</param>
        /// <param name="returnedBy"></param>
        /// <exception cref="Exceptions.Business.EntityNotFoundException{Issue}">Thrown when the issue is not found.</exception>
        /// <exception cref="Exceptions.Business.BookAlreadyReturnedException">Thrown when the book has already been returned.</exception>
        /// <returns></returns>
        Task Return(string id, UserInfoDTO returnedBy);
    }
}