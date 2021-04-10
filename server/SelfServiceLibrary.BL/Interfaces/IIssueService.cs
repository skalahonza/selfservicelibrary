using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.Issue;
using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.BL.Responses;

namespace SelfServiceLibrary.BL.Interfaces
{
    public interface IIssueService
    {
        Task<BorrowResponse> Borrow(UserInfoDTO issuedTo, IssueCreateDTO details, UserInfoDTO? issuedBy = null);
        Task<List<IssueListlDTO>> GetAll(int page, int pageSize, IEnumerable<(string column, ListSortDirection direction)>? sortings = null);
        Task<List<IssueListlDTO>> GetAll(string username);
        Task<List<IssueListlDTO>> GetBookIssues(string departmentNumber);
        Task<long> GetTotalCount(bool estimated = true);
        Task<ReturnResponse> Return(string id, UserInfoDTO returnedBy);
    }
}