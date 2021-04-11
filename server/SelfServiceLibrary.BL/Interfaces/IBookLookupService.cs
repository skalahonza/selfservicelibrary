using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.Book;

namespace SelfServiceLibrary.BL.Interfaces
{
    public interface IBookLookupService
    {
        Task<bool> FillData(BookDetailDTO book);
    }
}
