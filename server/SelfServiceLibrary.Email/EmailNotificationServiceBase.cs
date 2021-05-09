using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.DTO.Issue;
using SelfServiceLibrary.BL.Interfaces;

namespace SelfServiceLibrary.Email
{
    public abstract class EmailNotificationServiceBase : INotificationService
    {
        private static readonly ConcurrentDictionary<string, string> Templates = new ConcurrentDictionary<string, string>();

        static EmailNotificationServiceBase()
        {
            // initialize notification message HTML templates
            foreach (var file in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates")))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var content = File.ReadAllText(file);
                Templates.TryAdd(name, content);
            }
        }

        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        protected EmailNotificationServiceBase(IUserService userService, IBookService bookService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
            _bookService = bookService;
        }

        protected abstract Task Send(string title, string message, IEnumerable<(string email, string name)> recipients);

        private Task Send(string title, string message, (string email, string name) recipient) =>
            Send(title, message, new[] { recipient });

        private string GetMessage(string template, Dictionary<string, object> dictionary)
        {
            var sb = new StringBuilder(Templates[template]);
            foreach (var item in dictionary)
            {
                var key = item.Key;
                if (item is { Value: string value })
                {
                    sb.Replace("{" + key + "}", value);
                }

                else if (item is { Value: IEnumerable<string> values })
                {
                    sb.Replace("{" + key + "}", string.Join(", ", values));
                }

                else if (item is { Value: int number })
                {
                    sb.Replace("{" + key + "}", number.ToString());
                }

                else if (item is { Value: double decimalNumber })
                {
                    sb.Replace("{" + key + "}", decimalNumber.ToString("F1"));
                }

                else if (item is { Value: null })
                {
                    sb.Replace("{" + key + "}", "?");
                }
            }

            return sb.ToString();
        }

        public async Task SendNewsletter(BookDetailDTO book)
        {
            var users = await _userService.GetAll();
            var dictionary = _mapper.Map<Dictionary<string, object>>(book);
            var message = GetMessage("Newsletter", dictionary);
            await Send("New book in a library", message, users.Select(x => (x.InfoEmail, x.InfoFullName)));
        }

        public async Task WatchdogNotify(string departmentNumber)
        {
            var users = await _bookService.GetWatchdogs(departmentNumber);
            if (users.Count > 0)
            {
                var book = await _bookService.GetDetail(departmentNumber);
                var dictionary = _mapper.Map<Dictionary<string, object>>(book);
                var message = GetMessage("Watchdog", dictionary);
                await Send("Book you were interested in is available again", message, users.Select(x => (x.Email, x.ToString())));
                await _bookService.ClearWatchdogs(departmentNumber); 
            }
        }

        public async Task IssueExpiresSoonNotify(IssueListDTO issue)
        {
            var book = await _bookService.GetDetail(issue.DepartmentNumber);
            var dictionary = _mapper.Map<Dictionary<string, object>>(book);            
            var message = GetMessage("IssueExpiresSoonNotify", dictionary);
            await Send("Issue is about to expire", message, (issue.IssuedTo.Email, issue.IssuedTo.ToString()));
        }

        public async Task IssueExpiredNotify(IssueListDTO issue)
        {
            var book = await _bookService.GetDetail(issue.DepartmentNumber);
            var dictionary = _mapper.Map<Dictionary<string, object>>(book);
            var message = GetMessage("IssueExpiredNotify", dictionary);
            await Send("Issue expired", message, (issue.IssuedTo.Email, issue.IssuedTo.ToString()));
        }
    }
}
