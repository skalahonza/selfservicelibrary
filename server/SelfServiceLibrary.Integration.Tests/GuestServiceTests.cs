using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using SelfServiceLibrary.BL.DTO.Guest;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.Integration.Tests.Helpers;

using Xunit;

namespace SelfServiceLibrary.Integration.Tests
{
    public class GuestServiceTests : IntegrationTestBase, IClassFixture<DbFixture>
    {
        public GuestServiceTests(DbFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Add()
        {
            // Arrange
            var di = Services.BuildServiceProvider();
            var guestService = di.GetRequiredService<IGuestService>();
            var guest = new GuestDTO
            {
                TitleBefore = "Ing.",
                FirstName = "John",
                LastName = "Doe",
                TitleAfter = "Ph. D.",
                Email = "john@doe.com",
                PhoneNumber = "123456789"
            };

            // Act
            await guestService.Add(guest);

            // Assert
            var guests = await guestService.GetAll();
            guests.Should().ContainSingle( x =>
                x.TitleBefore == guest.TitleBefore &&
                x.FirstName == guest.FirstName &&
                x.LastName == guest.LastName &&
                x.TitleAfter == guest.TitleAfter &&
                x.Email == guest.Email &&
                x.PhoneNumber == guest.PhoneNumber
            );
        }

        [Fact]
        public async Task Delete()
        {
            // Arrange
            var di = Services.BuildServiceProvider();
            var guestService = di.GetRequiredService<IGuestService>();
            var guest = new GuestDTO
            {
                TitleBefore = "Ing.",
                FirstName = "Jane",
                LastName = "Doe",
                TitleAfter = "Ph. D.",
                Email = "jane@doe.com",
                PhoneNumber = "123456789"
            };

            // Act
            await guestService.Add(guest);
            var guests = await guestService.GetAll();
            await guestService.Delete(guests.Find(x => x.Email == guest.Email).Id);

            // Assert
            guests = await guestService.GetAll();
            guests.Should().NotContain(x =>
               x.TitleBefore == guest.TitleBefore &&
               x.FirstName == guest.FirstName &&
               x.LastName == guest.LastName &&
               x.TitleAfter == guest.TitleAfter &&
               x.Email == guest.Email &&
               x.PhoneNumber == guest.PhoneNumber
            );
        }

        [Theory]
        [InlineData("skála", true)]
        [InlineData("novák", true)]
        [InlineData("shouldnotfindanything", false)]
        public async Task Suggest(string term, bool shouldFound)
        {
            // Arrange
            var di = Services.BuildServiceProvider();
            var guestService = di.GetRequiredService<IGuestService>();

            // Act
            var suggestions = await guestService.Suggest(term);

            // Assert
            if (shouldFound)
            {
                suggestions.Should().HaveCount(1);
                suggestions.First().ToString().Should().ContainEquivalentOf(term);
            }
            else
            {
                suggestions.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task Update()
        {
            // Arrange
            var di = Services.BuildServiceProvider();
            var guestService = di.GetRequiredService<IGuestService>();
            var guest = new GuestDTO
            {
                FirstName = "Eve",
                LastName = "Smith",
                Email = "eve@doe.com",
            };
            await guestService.Add(guest);

            // Act
            var guests = await guestService.GetAll();
            var id = guests.Find(x => x.Email == guest.Email).Id;
            guest.Id = id;
            guest.FirstName = "Bob";
            guest.Email = "bob@doe.com";
            await guestService.Update(guest);

            // Assert
            guests = await guestService.GetAll();
            var updatedGuest = guests.Find(x => x.Id == id);
            updatedGuest.FirstName.Should().Be(guest.FirstName);
            updatedGuest.LastName.Should().Be(guest.LastName);
            updatedGuest.Email.Should().Be(guest.Email);
        }
    }
}
