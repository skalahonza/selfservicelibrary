﻿using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.DTO.Issue;
using SelfServiceLibrary.BL.Interfaces;

namespace SelfServiceLibrary.API.Services
{
    public class NullNotificationService : INotificationService
    {
        public Task IssueExpiresSoonNotify(IssueListDTO issue) =>
            Task.CompletedTask;

        public Task SendNewsletter(BookDetailDTO book) =>
            Task.CompletedTask;

        public Task WatchdogNotify(string departmentNumber) =>
            Task.CompletedTask;
    }
}
