﻿using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

using SelfServiceLibrary.BL.DTO.Guest;
using SelfServiceLibrary.BL.DTO.Issue;
using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.BL.Interfaces;

using Xunit;

namespace SelfServiceLibrary.Integration.Tests.Helpers
{
    public class DbFixture : IAsyncLifetime
    {
        public const string DB = "mongodb://root:rootpassword@localhost:27017";

        private bool seeded;
        public string DbName { get; } = $"test_db_{Guid.NewGuid()}";

        public Task InitializeAsync() =>
            Task.CompletedTask;

        public Task DisposeAsync()
        {
            var client = new MongoClient(DB);
            return client.DropDatabaseAsync(DbName);
        }

        public async Task Seed(IServiceCollection services)
        {
            if (seeded)
                return;

            seeded = true;

            var di = services.BuildServiceProvider();
            var bookService = di.GetRequiredService<IBookService>();
            var issueService = di.GetRequiredService<IIssueService>();
            var userService = di.GetRequiredService<IUserService>();
            var guestService = di.GetRequiredService<IGuestService>();

            var csv = File.OpenRead("Data/51-OstatniGL.csv");
            await bookService.ImportCsv(csv);
            csv.Dispose();

            // user
            await userService.UpdateInfo("skalaja7", new UserInfoDTO
            {
                Email = "skalaja7@fel.cvut.cz",
                FirstName = "Jan",
                LastName = "Skála",
            });

            // geusts
            var guest1 = new GuestDTO
            {
                FirstName = "Jan",
                LastName = "Skála",
            };
            await guestService.Add(guest1);
            var guest2 = new GuestDTO
            {
                TitleBefore = "Ing.",
                FirstName = "Petr",
                LastName = "Novák",
                TitleAfter = "Ph. D.",
            };
            await guestService.Add(guest2);

            // borrowed books
            await issueService.Borrow(new IssueCreateDTO
            {
                DepartmentNumber = "GL-00002",
                ExpiryDate = DateTime.Now.AddDays(365)
            });

            await issueService.Borrow(new IssueCreateDTO
            {
                DepartmentNumber = "GL-00184",
                ExpiryDate = DateTime.Now.AddDays(365)
            });

            // returned book
            var issue = await issueService.Borrow(new IssueCreateDTO
            {
                DepartmentNumber = "GL-00011",
                ExpiryDate = DateTime.Now.AddDays(365)
            });
            await issueService.Return(issue.Id);

            issue = await issueService.BorrowTo(new IssueCreateDTO
            {
                DepartmentNumber = "GL-00040",
                ExpiryDate = DateTime.Now.AddDays(365)
            }, new UserInfoDTO
            {
                Email = "skalaja7@fel.cvut.cz",
                Username = "skalaja7",
                FirstName = "Jan",
                LastName = "Skála",
            });
            await issueService.Return(issue.Id);

            // book statuses
        }
    }
}
