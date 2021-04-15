using System;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;
using FluentAssertions.Common;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;

using Moq;

using SelfServiceLibrary.BG;
using SelfServiceLibrary.BL.DTO.Issue;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.BL.Services;
using SelfServiceLibrary.Integration.Tests.Extensions;
using SelfServiceLibrary.Integration.Tests.Helpers;

using Xunit;

namespace SelfServiceLibrary.Integration.Tests
{
    public class IssuesReminderTests : IntegrationTestBase, IClassFixture<DbFixture>
    {
        public IssuesReminderTests(DbFixture fixture)
            : base(fixture)
        {
            var env = new HostingEnvironment { EnvironmentName = Environments.Development };
            Program.ConfigureServices(Services, Configuration, env);

            // Authorization
            Services.Replace<IAuthorizationContext, PermissiveContext>(ServiceLifetime.Singleton);

            Services.AddScoped<IIssueService, IssueService>();
            Services.AddSingleton<IssuesReminder>();
        }

        [Fact]
        public async Task ShouldNotifyIssuesNearExpiration()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();
            var mock = new Mock<INotificationService>();
            mock.Setup(x => x.IssueExpiresSoonNotify(It.IsAny<IssueListDTO>()))
                .Returns((IssueListDTO issue) =>
                {
                    issue.Should().NotBeNull();
                    issue.DepartmentNumber.Should().Be("GL-00021");
                    tokenSource.Cancel();
                    return Task.CompletedTask;
                });

            Services.Replace(s => mock.Object, ServiceLifetime.Singleton);

            var di = Services.BuildServiceProvider();
            var worker = di.GetRequiredService<IssuesReminder>();

            var issueService = di.GetRequiredService<IIssueService>();
            await issueService.Borrow(new IssueCreateDTO
            {
                DepartmentNumber = "GL-00021",
                ExpiryDate = DateTime.Today.AddDays(1)
            });

            // Act
            await worker.StartAsync(tokenSource.Token);
            await Task.Delay(TimeSpan.FromSeconds(10));

            // Assert
            mock.Verify(x => x.IssueExpiresSoonNotify(It.IsAny<IssueListDTO>()), Times.Once);
        }

        public async Task ShouldNotifyExpiredIssues()
        {
            // Arrange
        }
    }
}
