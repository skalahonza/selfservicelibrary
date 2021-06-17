using System;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Moq;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.DTO.Issue;
using SelfServiceLibrary.BL.Exceptions;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.BL.Services;
using SelfServiceLibrary.Integration.Tests.Extensions;
using SelfServiceLibrary.Integration.Tests.Helpers;

using Xunit;

namespace SelfServiceLibrary.Integration.Tests
{
    public class BookServiceTests : IntegrationTestBase, IClassFixture<DbFixture>
    {
        public BookServiceTests(DbFixture fixture) : base(fixture)
        {
        }

        private void AlterAuthContext(bool canManageContent)
        {
            var mock = new Mock<IAuthorizationContext>();
            mock.Setup(x => x.CanManageContent()).ReturnsAsync(canManageContent);
            mock.Setup(x => x.GetUserInfo()).Returns(new PermissiveContext().GetUserInfo());
            Services.Replace(s => mock.Object, ServiceLifetime.Singleton);
        }

        [Theory]
        [InlineData(5000)]

        public async Task PageSizeShouldNotBeUnlimited(int pageSize)
        {
            // Arrange
            var di = Services.BuildServiceProvider();
            var bookService = di.GetRequiredService<IBookService>();

            // Act
            var books = await bookService.GetAll(1, pageSize, new BookFilter());

            // Assert
            books.Data.Count.Should().BeLessOrEqualTo(BookService.MAX_PAGESIZE);
        }

        [Theory]
        [InlineData(true, null)]
        [InlineData(false, typeof(AuthorizationException))]
        public async Task Create(bool canManageContent, Type exception)
        {
            // Arrange
            AlterAuthContext(canManageContent);
            var di = Services.BuildServiceProvider();
            var bookService = di.GetRequiredService<IBookService>();

            // Act
            var departmentNumber = Guid.NewGuid().ToString();
            Task act() => bookService.Create(new BookAddDTO
            {
                DepartmentNumber = departmentNumber,
                PublicationType = "Book"
            });

            // Assert
            if (exception == null)
            {
                await act();
                var book = await bookService.GetDetail(departmentNumber);
                book.IsAvailable.Should().BeTrue();
                book.PublicationType.Should().Be("Book");
            }
            else
            {
                await Assert.ThrowsAsync(exception, act);
            }
        }

        [Theory]
        [InlineData(true, null)]
        [InlineData(false, typeof(AuthorizationException))]
        public async Task Update(bool canManageContent, Type exception)
        {
            // Arrange
            var departmentNumber = "GL-00184";
            AlterAuthContext(canManageContent);
            var di = Services.BuildServiceProvider();
            var bookService = di.GetRequiredService<IBookService>();
            var issueService = di.GetRequiredService<IIssueService>();

            // Act
            var data = new BookEditDTO
            {
                Name = "New name"
            };
            Task act() => bookService.Update(departmentNumber, data);

            // Assert
            if (exception == null)
            {
                await act();
                var detail = await bookService.GetDetail(departmentNumber);
                detail.Name.Should().Be(data.Name);

                var issues = await issueService.GetBookIssues(departmentNumber);
                foreach (var issue in issues)
                {
                    issue.BookName.Should().Be(data.Name);
                }
            }
            else
            {
                await Assert.ThrowsAsync(exception, act);
            }
        }

        [Fact]
        public async Task AddOrUpdateReview()
        {
            // Arrange
            var departmentNumber = "GL-00257";
            var username = (await new PermissiveContext().GetUserInfo()).Username;
            var di = Services.BuildServiceProvider();
            var bookService = di.GetRequiredService<IBookService>();
            var issueService = di.GetRequiredService<IIssueService>();
            await issueService.Borrow(new IssueCreateDTO
            {
                DepartmentNumber = departmentNumber,
                ExpiryDate = DateTime.Now.AddDays(365)
            });

            // Act
            await bookService.AddOrUpdateReview(new BookReviewDTO
            {
                DepartmentNumber = departmentNumber,
                Username = username,
                Value = 5
            });

            // Assert
            var hasReviewed = await bookService.HasReviewed(departmentNumber,username);
            hasReviewed.Should().BeTrue();

            var book = await bookService.GetDetail(departmentNumber);
            book.ReviewsCount.Should().Be(1);
            book.ReviewsAvg.Should().Be(5);
        }

        [Fact]
        public async Task RegisterWatchdog()
        {
            // Arrange
            var departmentNumber = "GL-00260";
            var username = (await new PermissiveContext().GetUserInfo()).Username;
            var mock = new Mock<INotificationService>();
            mock.Setup(x => x.WatchdogNotify(It.IsAny<string>()))
                .Returns((string number) =>
                {
                    number.Should().Be(departmentNumber);
                    return Task.CompletedTask;
                });
            Services.Replace(s => mock.Object, ServiceLifetime.Singleton);
            var di = Services.BuildServiceProvider();
            var bookService = di.GetRequiredService<IBookService>();
            var issueService = di.GetRequiredService<IIssueService>();
            var issue = await issueService.Borrow(new IssueCreateDTO
            {
                DepartmentNumber = departmentNumber,
                ExpiryDate = DateTime.Now.AddDays(365)
            });

            // Act
            await bookService.RegisterWatchdog(departmentNumber);
            await issueService.Return(issue.Id);

            // Assert
            var watchDogs = await bookService.GetWatchdogs(departmentNumber);
            watchDogs.Should().Contain(x => x.Username == username);
            mock.Verify(x => x.WatchdogNotify(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ClearWatchdogs()
        {
            // Arrange
            var departmentNumber = "GL-00261";
            var username = (await new PermissiveContext().GetUserInfo()).Username;
            var mock = new Mock<INotificationService>();
            mock.Setup(x => x.WatchdogNotify(It.IsAny<string>())).Returns(Task.CompletedTask);
            Services.Replace(s => mock.Object, ServiceLifetime.Singleton);
            var di = Services.BuildServiceProvider();
            var bookService = di.GetRequiredService<IBookService>();
            var issueService = di.GetRequiredService<IIssueService>();
            var issue = await issueService.Borrow(new IssueCreateDTO
            {
                DepartmentNumber = departmentNumber,
                ExpiryDate = DateTime.Now.AddDays(365)
            });

            // Act
            await bookService.RegisterWatchdog(departmentNumber);
            await bookService.ClearWatchdogs(departmentNumber);
            await issueService.Return(issue.Id);

            // Assert
            var watchDogs = await bookService.GetWatchdogs(departmentNumber);
            watchDogs.Should().NotContain(x => x.Username == username);
            mock.Verify(x => x.WatchdogNotify(It.IsAny<string>()), Times.Once);
        }
    }
}
