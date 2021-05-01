using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.DTO.User;
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

        private readonly List<UserListDTO> Users = new()
        {
            new()
            {
                InfoEmail = "skalaja7@fel.cvut.cz",
                InfoFullName = "Jan Skála"
            },
            new()
            {
                InfoEmail = "novakpe@fel.cvut.cz",
                InfoFullName = "Petr Novák"
            },
        };

        private readonly List<UserInfoDTO> Watchdogs = new()
        {
            new()
            {
                Email = "skalaja7@fel.cvut.cz",
                FullName = "Jan Skála"
            },
            new()
            {
                Email = "novakpe@fel.cvut.cz",
                FullName = "Petr Novák"
            },
            new()
            {
                Email = "doejohn@fel.cvut.cz",
                FullName = "John Doe"
            },
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
            mock.Setup(x => x.GetAll()).ReturnsAsync(Users);
            return mock.Object;
        }

        private IBookService MockBookService()
        {
            var mock = new Mock<IBookService>();
            mock.Setup(x => x.GetDetail(It.IsAny<string>())).ReturnsAsync(Book);
            mock.Setup(x => x.GetWatchdogs(It.IsAny<string>())).ReturnsAsync(Watchdogs);
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
                    title.Should().Be("Issue expired");

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
        public async Task IssueExpiresSoonNotify()
        {
            // Arrange
            var issuedToEmail = "skalaja7@fel.cvut.cz";
            var issuedToName = "skalaja7@fel.cvut.cz";
            var service = new MockNotificationService(MockUserService(), MockBookService(), _mapper)
            {
                // Assert
                Act = (string title, string message, IEnumerable<(string email, string name)> recipients) =>
                {
                    title.Should().Be("Issue is about to expire");

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
            await service.IssueExpiresSoonNotify(new()
            {
                IssuedTo = new() { Email = issuedToEmail, FullName = issuedToName }
            });
        }

        [Fact]
        public async Task SendNewsletter()
        {
            // Arrange
            var service = new MockNotificationService(MockUserService(), MockBookService(), _mapper)
            {
                // Assert
                Act = (string title, string message, IEnumerable<(string email, string name)> recipients) =>
                {
                    title.Should().Be("New book in a library");

                    message.Should().Contain(Book.Name);
                    message.Should().Contain(Book.Author);
                    message.Should().ContainAll(Book.CoAuthors);

                    var expectedRecipients = Users.Select(x => (x.InfoEmail, x.InfoFullName));
                    recipients.Should().BeEquivalentTo(expectedRecipients);
                }
            };

            // Act
            await service.SendNewsletter(Book);
        }

        [Fact]
        public async Task WatchdogNotify()
        {
            // Arrange
            var service = new MockNotificationService(MockUserService(), MockBookService(), _mapper)
            {
                // Assert
                Act = (string title, string message, IEnumerable<(string email, string name)> recipients) =>
                {
                    title.Should().Be("Book you were interested in is available again");

                    message.Should().Contain(Book.Name);
                    message.Should().Contain(Book.Author);
                    message.Should().ContainAll(Book.CoAuthors);

                    var expectedRecipients = Watchdogs.Select(x => (x.Email, x.FullName));
                    recipients.Should().BeEquivalentTo(expectedRecipients);
                }
            };

            // Act
            await service.WatchdogNotify(Book.DepartmentNumber);
        }
    }
}
