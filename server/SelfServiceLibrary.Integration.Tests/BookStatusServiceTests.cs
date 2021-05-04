
using System;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using SelfServiceLibrary.BL.DTO.BookStatus;
using SelfServiceLibrary.BL.Exceptions;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.BL.Services;
using SelfServiceLibrary.DAL.Entities;
using SelfServiceLibrary.Integration.Tests.Helpers;

using Xunit;

namespace SelfServiceLibrary.Integration.Tests
{
    public class BookStatusServiceTests : IntegrationTestBase, IClassFixture<DbFixture>
    {
        public BookStatusServiceTests(DbFixture fixture) : base(fixture)
        {
            Services.AddScoped<IBookStatusService, BookStatusService>();
        }

        [Theory]
        [InlineData("Status1", true, true)]
        [InlineData("Status2", false, true)]
        [InlineData("Status3", true, false)]
        [InlineData("Status4", false, false)]
        public async Task Create(string name, bool isVisible, bool canBeBorrowed)
        {
            // Arrange
            var bookStatus = new BookStatusCreateDTO
            {
                Name = name,
                IsVisible = isVisible,
                CanBeBorrowed = canBeBorrowed
            };
            var di = Services.BuildServiceProvider();
            var bookStatusService = di.GetRequiredService<IBookStatusService>();

            // Act
            await bookStatusService.Create(bookStatus);

            // Assert
            var statuses = await bookStatusService.GetAll();
            statuses.Should().ContainSingle(x =>
                x.Name == name &&
                x.IsVisible == isVisible &&
                x.CanBeBorrowed == canBeBorrowed
            );
        }

        [Fact]
        public async Task ShouldNotCreateDuplicity()
        {
            // Arrange
            var bookStatus = new BookStatusCreateDTO
            {
                Name = "Duplicity",
                IsVisible = true,
                CanBeBorrowed = true
            };
            var di = Services.BuildServiceProvider();
            var bookStatusService = di.GetRequiredService<IBookStatusService>();
            await bookStatusService.Create(bookStatus);

            // Act
            Func<Task> act = () => bookStatusService.Create(bookStatus);

            // Assert
            await act.Should().ThrowAsync<StatusAlreadyExistsException>();
        }

        [Fact]
        public async Task Remove()
        {
            // Arrange
            var bookStatus = new BookStatusCreateDTO
            {
                Name = "ToBeRemoved",
                IsVisible = false,
                CanBeBorrowed = false
            };
            var di = Services.BuildServiceProvider();
            var bookStatusService = di.GetRequiredService<IBookStatusService>();
            var bookService = di.GetRequiredService<IBookService>();
            await bookStatusService.Create(bookStatus);
            await bookService.Create(new()
            {
                DepartmentNumber = "Toberemoved1",
                PublicationType = "Book"
            });
            await bookService.Create(new()
            {
                DepartmentNumber = "Toberemoved2",
                PublicationType = "Book"
            });
            await bookService.Update("Toberemoved1", new() { StatusName = bookStatus.Name });
            await bookService.Update("Toberemoved2", new() { StatusName = bookStatus.Name });

            // Act
            await bookStatusService.Remove(bookStatus.Name);

            // Assert
            var statuses = await bookStatusService.GetAll();
            statuses.Should().NotContain(x => x.Name == bookStatus.Name);

            var book1 = await bookService.GetDetail("Toberemoved1");
            book1.Status.Name.Should().Be(new BookStatus().Name, because: "Book should fallback to default status.");

            var book2 = await bookService.GetDetail("Toberemoved2");
            book2.Status.Name.Should().Be(new BookStatus().Name, because: "Book should fallback to default status.");
        }

        [Fact]
        public async Task Update()
        {
            // Arrange
            var bookStatus = new BookStatusCreateDTO
            {
                Name = "ToBeUpdated",
                IsVisible = false,
                CanBeBorrowed = false
            };
            var di = Services.BuildServiceProvider();
            var bookStatusService = di.GetRequiredService<IBookStatusService>();
            var bookService = di.GetRequiredService<IBookService>();
            await bookStatusService.Create(bookStatus);
            await bookService.Create(new()
            {
                DepartmentNumber = "TobeUpdated1",
                PublicationType = "Book"
            });
            await bookService.Create(new()
            {
                DepartmentNumber = "TobeUpdated2",
                PublicationType = "Book"
            });
            await bookService.Update("TobeUpdated1", new() { StatusName = bookStatus.Name });
            await bookService.Update("TobeUpdated2", new() { StatusName = bookStatus.Name });

            // Act
            await bookStatusService.Update(bookStatus.Name, new()
            {
                IsVisible = true,
                CanBeBorrowed = true
            });

            // Assert
            var statuses = await bookStatusService.GetAll();
            statuses.Should().ContainSingle(x =>
                x.Name == bookStatus.Name &&
                x.IsVisible == true &&
                x.CanBeBorrowed == true
            );

            var book1 = await bookService.GetDetail("TobeUpdated1");
            book1.Status.Name.Should().Be(bookStatus.Name);
            book1.Status.IsVisible.Should().BeTrue();
            book1.Status.CanBeBorrowed.Should().BeTrue();

            var book2 = await bookService.GetDetail("TobeUpdated2");
            book2.Status.Name.Should().Be(bookStatus.Name);
            book2.Status.IsVisible.Should().BeTrue();
            book2.Status.CanBeBorrowed.Should().BeTrue();
        }
    }
}
