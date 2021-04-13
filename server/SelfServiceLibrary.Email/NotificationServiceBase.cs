using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.BL.Interfaces;

namespace SelfServiceLibrary.Email
{
    public abstract class NotificationServiceBase : INotificationService
    {
        private static readonly ConcurrentDictionary<string, string> Templates = new ConcurrentDictionary<string, string>();

        static NotificationServiceBase()
        {
            // initialize notification message HTML templates
            foreach (var file in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates")))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var content = File.ReadAllText(file);
                Templates.TryAdd(name, content);
            }
        }

        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        protected NotificationServiceBase(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        protected abstract Task Send(string title, string message, IEnumerable<UserListDTO> recipients);

        protected Task Send(string title, string message, UserListDTO recipient) =>
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
            await Send("New book in a library", message, users);
        }
    }
}
