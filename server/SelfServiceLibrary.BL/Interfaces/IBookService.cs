using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.BL.Responses;
using SelfServiceLibrary.BL.ViewModels;
using SelfServiceLibrary.DAL.Enums;
using SelfServiceLibrary.DAL.Filters;

namespace SelfServiceLibrary.BL.Interfaces
{
    public interface IBookService
    {
        Task<CreateBookResponse> Create(BookAddDTO data);
        Task<DeleteBookResponse> Delete(string departmentNumber);
        Task DeleteAll();
        Task<bool> Exists(string departmentNumber);
        Task ExportCsv(Stream csv, IBooksFilter filter, bool leaveOpen = false);
        Task<List<BookSearchDTO>> Fulltext(string searchedTerm);
        Task<PaginatedVM<BookListDTO>> GetAll(int page, int pageSize, IBooksFilter filter, IEnumerable<(string column, ListSortDirection direction)>? sortings = null);
        Task<BookListDTO> GetByNFC(string serNumNFC);
        Task<BookDetailDTO?> GetDetail(string departmentNumber);
        Task<Dictionary<string, int>> GetFormTypes();
        Task<Dictionary<string, int>> GetPublicationTypes(IBooksFilter filter);
        Task<Dictionary<string, int>> GetPublicationTypes(ISet<Role> userRoles);
        Task<int> GetTotalCount(IBooksFilter filter);
        Task ImportCsv(Stream csv, UserInfoDTO enteredBy);
        Task Update(string departmentNumber, BookEditDTO data);
    }
}