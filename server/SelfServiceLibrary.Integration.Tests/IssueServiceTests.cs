using System;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Moq;

using SelfServiceLibrary.BL.DTO.Issue;
using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.BL.Exceptions.Authorization;
using SelfServiceLibrary.BL.Exceptions.Business;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.DAL.Entities;
using SelfServiceLibrary.Integration.Tests.Extensions;
using SelfServiceLibrary.Integration.Tests.Helpers;

using Xunit;

namespace SelfServiceLibrary.Integration.Tests
{
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
            if (exception == null)
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

        [Theory]
        [InlineData("GL-00005", true, null)]
        [InlineData("GL-00002", true, typeof(BookIsBorrowedException))]
        [InlineData("GL-error", true, typeof(EntityNotFoundException<Book>))]
        [InlineData("GL-00006", false, typeof(AuthorizationException))]
        public async Task BorrowTo(string departmentNumber, bool canBorrowTo, Type exception)
        {
            // Arrange
            var mock = new Mock<IAuthorizationContext>();
            mock.Setup(x => x.CanBorrowTo()).ReturnsAsync(canBorrowTo);
            mock.Setup(x => x.GetUserInfo()).Returns(new PermissiveContext().GetUserInfo());
            Services.Replace(s => mock.Object, ServiceLifetime.Singleton);

            var di = Services.BuildServiceProvider();
            var issueService = di.GetRequiredService<IIssueService>();
            var user = new UserInfoDTO
            {
                Email = "skalaja7@fel.cvut.cz",
                Username = "skalaja7",
                FirstName = "Jan",
                LastName = "Skála",
            };

            // Act
            Func<Task<IssueDetailDTO>> act = () => issueService.BorrowTo(new IssueCreateDTO
            {
                DepartmentNumber = departmentNumber,
                ExpiryDate = DateTime.Now.AddDays(7)
            }, user);

            // Assert
            if (exception == null)
            {
                var issue = await act();
                issue.DepartmentNumber.Should().Be(departmentNumber);
                issue.IsReturned.Should().BeFalse();
                issue.IssuedTo.Should().BeEquivalentTo(user);
            }
            else
            {
                await Assert.ThrowsAsync(exception, act);
            }
        }
    }
}
