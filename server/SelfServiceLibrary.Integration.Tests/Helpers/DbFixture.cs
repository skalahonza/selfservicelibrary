using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

using SelfServiceLibrary.BL.DTO.Issue;
using SelfServiceLibrary.BL.Interfaces;

using Xunit;

namespace SelfServiceLibrary.Integration.Tests.Helpers
{
    public class DbFixture : IAsyncLifetime
    {
        public const string DB = "mongodb://root:rootpassword@localhost:27017";

        private bool seeded;

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

            var csv = File.OpenRead("Data/51-OstatniGL.csv");
            await bookService.ImportCsv(csv);
            csv.Dispose();

            // borrowed book
            await issueService.Borrow(new IssueCreateDTO
            {
                DepartmentNumber = "GL-00002",
                ExpiryDate = DateTime.Now.AddDays(365)
            });

            // returned book
            var issue = await issueService.Borrow(new IssueCreateDTO
            {
                DepartmentNumber = "GL-00011",
                ExpiryDate = DateTime.Now.AddDays(365)
            });
            await issueService.Return(issue.Id);
        }

        public string DbName { get; } = $"test_db_{Guid.NewGuid()}";
    }
}
