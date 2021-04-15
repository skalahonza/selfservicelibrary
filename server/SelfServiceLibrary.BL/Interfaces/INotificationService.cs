using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.DTO.Issue;

namespace SelfServiceLibrary.BL.Interfaces
{
    public interface INotificationService
    {
        /// <summary>
        /// Notify all users that a new books is available in the library
        /// </summary>
        /// <param name="book">Book for the newsletter</param>
        /// <returns></returns>
        Task SendNewsletter(BookDetailDTO book);

        /// <summary>
        /// Notify all users who are watching for the book availability
        /// </summary>
        /// <param name="departmentNumber">Department number of the book that has been just returned</param>
        /// <returns></returns>
        Task WatchdogNotify(string departmentNumber);

        /// <summary>
        /// Notify a person who has borrowed the book included in the issue that, the issue is about to expire.
        /// </summary>
        /// <param name="issue"></param>
        /// <returns></returns>
        Task IssueExpiresSoonNotify(IssueListDTO issue);
    }
}
