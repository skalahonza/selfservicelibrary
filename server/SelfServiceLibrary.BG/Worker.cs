using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MongoDB.Driver;

using SelfServiceLibrary.BG.Filters;
using SelfServiceLibrary.BL.DTO.Issue;
using SelfServiceLibrary.BL.Extensions;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.DAL;
using SelfServiceLibrary.DAL.Entities;
using SelfServiceLibrary.DAL.Queries;

namespace SelfServiceLibrary.BG
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                using var scope = _serviceScopeFactory.CreateScope();
                var provider = scope.ServiceProvider;

                var mapper = provider.GetRequiredService<IMapper>();
                var dbContext = provider.GetRequiredService<MongoDbContext>();
                var notificationService = provider.GetRequiredService<INotificationService>();

                var issues = dbContext
                    .Issues
                    .AsQueryable()
                    .Filter(new NotReturnedIssues())
                    .ProjectTo<Issue, IssueListDTO>(mapper)
                    .AsAsyncEnumerable();

                await foreach(var issue in issues)
                {
                    Console.WriteLine($"{issue.BookName} {issue.IssuedTo.Email}");
                }

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
