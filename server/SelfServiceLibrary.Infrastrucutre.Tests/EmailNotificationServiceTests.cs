using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.DTO.Issue;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.Mapping;
using SelfServiceLibrary.Mapping.Profiles;

using Xunit;

namespace SelfServiceLibrary.Infrastrucutre.Tests
{
    public class EmailNotificationServiceTests
    {
        private readonly IMapper _mapper;

        private readonly BookDetailDTO Book = new()
        {
            Name = "Book Name",
            DepartmentNumber = "Book DepartmentNumber",
            Author = "Book Author",
            CoAuthors = new() { "Author1", "Author2" }
        };

        public EmailNotificationServiceTests()
        {
            var services = new ServiceCollection();
            services.AddAutoMapper(typeof(BookProfile));
            services.AddScoped<IMapper, AutoMapperAdapter>();

            _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
        }

        private IUserService MockUserService()
        {
            var mock = new Mock<IUserService>();
            return mock.Object;
        }

        private IBookService MockBookService()
        {
            var mock = new Mock<IBookService>();
            mock.Setup(x => x.GetDetail(It.IsAny<string>())).ReturnsAsync(Book);
            return mock.Object;
        }


        [Fact]
        public async Task IssueExpiredNotify()
        {
            // Arrange
            var issuedToEmail = "skalaja7@fel.cvut.cz";
            var issuedToName = "skalaja7@fel.cvut.cz";
            var service = new MockNotificationService(MockUserService(), MockBookService(), _mapper)
            {
                // Assert
                Act = (string title, string message, IEnumerable<(string email, string name)> recipients) =>
                {
                    message.Should().Contain(Book.Name);
                    message.Should().Contain(Book.DepartmentNumber);
                    message.Should().Contain(Book.Author);
                    message.Should().ContainAll(Book.CoAuthors);

                    recipients.Should().HaveCount(1);
                    recipients.First().email.Should().Be(issuedToEmail);
                    recipients.First().name.Should().Be(issuedToName);
                }
            };

            // Act
            await service.IssueExpiredNotify(new()
            {
                IssuedTo = new() { Email = issuedToEmail, FullName = issuedToName }
            });
        }

        [Fact]
        public Task IssueExpiresSoonNotify()
        {
            throw new System.NotImplementedException();
        }

        [Fact]
        public Task SendNewsletter()
        {
            throw new System.NotImplementedException();
        }

        [Fact]
        public Task WatchdogNotify()
        {
            throw new System.NotImplementedException();
        }
    }
}
