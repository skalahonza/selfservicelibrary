using System.Collections.Generic;
using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.BookStatus;

namespace SelfServiceLibrary.BL.Interfaces
{
    public interface IBookStatusService
    {
        /// <summary>
        /// Creates a new book status
        /// </summary>
        /// <param name="bookStatus"></param>
        /// <exception cref="Exceptions.Business.StatusAlreadyExistsException">Thrown when status with the same name already exists.</exception>
        /// <returns></returns>
        Task Create(BookStatusCreateDTO bookStatus);
        Task<List<BookStatusListDTO>> GetAll();
        Task Remove(string name);
        Task Update(string name, BookStatusUpdateDTO bookStatus);
    }
}