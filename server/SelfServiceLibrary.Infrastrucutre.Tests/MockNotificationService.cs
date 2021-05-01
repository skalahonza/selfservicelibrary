using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.Email;

namespace SelfServiceLibrary.Infrastrucutre.Tests
{
    public class MockNotificationService : EmailNotificationServiceBase
    {
        public MockNotificationService(IUserService userService, IBookService bookService, IMapper mapper) 
            : base(userService, bookService, mapper)
        {
        }

        public Action<string, string, IEnumerable<(string email, string name)>> Act { get; set; }

        protected override Task Send(string title, string message, IEnumerable<(string email, string name)> recipients)
        {
            Act(title, message, recipients);
            return Task.CompletedTask;
        }
    }
}
