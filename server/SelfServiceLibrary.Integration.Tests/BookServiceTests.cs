using System;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Moq;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.Exceptions;
using SelfServiceLibrary.BL.Interfaces;
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

        [Theory]
        [InlineData(true, null)]
        [InlineData(false, typeof(AuthorizationException))]
        public async Task Create(bool canManageContent, Type exception)
        {
            // Arrange
            var mock = new Mock<IAuthorizationContext>();
            mock.Setup(x => x.CanManageContent()).ReturnsAsync(canManageContent);
            mock.Setup(x => x.GetUserInfo()).Returns(new PermissiveContext().GetUserInfo());
            Services.Replace(s => mock.Object, ServiceLifetime.Singleton);

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

        public async Task Update()
        {

        }

        public async Task AddOrUpdateReview()
        {

        }

        public async Task RegisterWatchdog()
        {

        }

        public async Task ClearWatchdogs()
        {

        }
    }
}
