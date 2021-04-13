using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.Interfaces;

namespace SelfServiceLibrary.API.Services
{
    public class NullNotificationService : INotificationService
    {
        public Task SendNewsletter(BookDetailDTO book) =>
            Task.CompletedTask;

        public Task WatchdogNotify(string departmentNumber) =>
            Task.CompletedTask;
    }
}
