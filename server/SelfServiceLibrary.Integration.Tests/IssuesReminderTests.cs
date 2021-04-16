using System;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;

using Moq;

using SelfServiceLibrary.BG;
using SelfServiceLibrary.BL.DTO.Issue;
using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.BL.Exceptions.Authorization;
using SelfServiceLibrary.BL.Exceptions.Business;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.BL.Services;
using SelfServiceLibrary.DAL.Entities;
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

        [Fact]
        public async Task ShouldNotifyExpiredIssues()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();
            var mock = new Mock<INotificationService>();
            mock.Setup(x => x.IssueExpiredNotify(It.IsAny<IssueListDTO>()))
                .Returns((IssueListDTO issue) =>
                {
                    issue.Should().NotBeNull();
                    issue.DepartmentNumber.Should().Be("GL-00047");
                    tokenSource.Cancel();
                    return Task.CompletedTask;
                });

            Services.Replace(s => mock.Object, ServiceLifetime.Singleton);

            var di = Services.BuildServiceProvider();
            var worker = di.GetRequiredService<IssuesReminder>();

            var issueService = di.GetRequiredService<IIssueService>();
            await issueService.Borrow(new IssueCreateDTO
            {
                DepartmentNumber = "GL-00047",
                ExpiryDate = DateTime.Today.AddDays(-10)
            });

            // Act
            await worker.StartAsync(tokenSource.Token);
            await Task.Delay(TimeSpan.FromSeconds(10));

            // Assert
            mock.Verify(x => x.IssueExpiredNotify(It.IsAny<IssueListDTO>()), Times.Once);
        }
    }

    public class IssueServiceTests : IntegrationTestBase, IClassFixture<DbFixture>
    {
        public IssueServiceTests(DbFixture fixture) : base(fixture)
        {            
        }

        [Theory]
        [InlineData("GL-00001", true, null)]
        [InlineData("GL-00002", true, typeof(BookIsBorrowedException))]
        [InlineData("GL-error", true, typeof(EntityNotFoundException<Book>))]
        [InlineData("GL-00003", false, typeof(AuthorizationException))]
        public async Task Borrow(string departmentNumber, bool canBorrow, Type exception)
        {
            // Arrange
            var mock = new Mock<IAuthorizationContext>();
            mock.Setup(x => x.CanBorrow()).ReturnsAsync(canBorrow);
            mock.Setup(x => x.GetUserInfo()).Returns(new PermissiveContext().GetUserInfo());
            Services.Replace(s => mock.Object, ServiceLifetime.Singleton);

            var di = Services.BuildServiceProvider();
            var issueService = di.GetRequiredService<IIssueService>();

            // Act
            Func<Task<IssueDetailDTO>> act = () => issueService.Borrow(new IssueCreateDTO
            {
                DepartmentNumber = departmentNumber,
                ExpiryDate = DateTime.Now.AddDays(7)
            });

            // Assert
            if(exception == null)
            {
                var issue = await act();
                issue.DepartmentNumber.Should().Be(departmentNumber);
                issue.IsReturned.Should().BeFalse();
                issue.IssuedTo.Should().BeEquivalentTo(await new PermissiveContext().GetUserInfo());
            }
            else
            {
                await Assert.ThrowsAsync(exception, act);
            }
        }
    }
}
