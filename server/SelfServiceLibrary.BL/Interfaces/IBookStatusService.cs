using System.Collections.Generic;
using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.BookStatus;
using SelfServiceLibrary.BL.Responses;

namespace SelfServiceLibrary.BL.Interfaces
{
    public interface IBookStatusService
    {
        Task<CreateStatusResponse> Create(BookStatusCreateDTO bookStatus);
        Task<List<BookStatusListDTO>> GetAll();
        Task Remove(string name);
        Task Update(string name, BookStatusUpdateDTO bookStatus);
    }
}