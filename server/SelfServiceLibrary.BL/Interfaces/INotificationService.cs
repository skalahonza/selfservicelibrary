using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.Book;

namespace SelfServiceLibrary.BL.Interfaces
{
    public interface INotificationService
    {
        Task SendNewsletter(BookDetailDTO book);
    }
}
