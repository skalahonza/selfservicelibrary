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
    /// <summary>
    /// Background service responsible for reminding users that they forgot to return a book or that the issue is about to expire. Runs every day.
    /// </summary>
    public class IssuesReminder : BackgroundService
    {
        private readonly ILogger<IssuesReminder> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public IssuesReminder(ILogger<IssuesReminder> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                using (var scope = _serviceScopeFactory.CreateScope())
                {
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

                    await foreach (var issue in issues)
                    {
                        _logger.LogInformation($"{issue.BookName} {issue.IssuedTo.Email}");

                        // issue expired
                        if (issue.ExpiryDate < DateTime.Today)
                        {
                            _logger.LogInformation("Issue has expired");
                            await notificationService.IssueExpiredNotify(issue);
                        }

                        // issue is about to expire
                        else if (issue.ExpiryDate < DateTime.Today.AddDays(7))
                        {
                            _logger.LogInformation("Issue is about to expire");
                            await notificationService.IssueExpiresSoonNotify(issue);
                        }
                    } 
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
