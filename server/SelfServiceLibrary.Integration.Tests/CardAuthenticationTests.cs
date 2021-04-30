using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using SelfServiceLibrary.BL.DTO.Card;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.BL.Services;
using SelfServiceLibrary.Card.Authentication.Extensions;
using SelfServiceLibrary.Card.Authentication.Services;
using SelfServiceLibrary.Integration.Tests.Helpers;

using Xunit;

namespace SelfServiceLibrary.Integration.Tests
{
    public class CardAuthenticationTests : IntegrationTestBase, IClassFixture<DbFixture>
    {
        public CardAuthenticationTests(DbFixture fixture) 
            : base(fixture)
        {
            Services.AddScoped<ICardService, CardService>();
            Services.Decorate<ICardService, AspNetCoreIdentityDecorator>();
            Services.AddCardAuthentication(Configuration.GetSection("Identity"));
        }

        private static Task<bool> InsertCard(ICardService service, string number) => 
            service.Add("skalaja7", new AddCardDTO
        {
            Name = "test",
            Number = number,
            Pin = "1234",
            PinConfirmation = "1234"
        });

        [Fact]
        public async Task AddCard()
        {
            // Arrange
            var di = Services.BuildServiceProvider();
            var service = di.GetRequiredService<ICardService>();

            // Act
            bool result = await InsertCard(service, "12345");

            // Assert
            result.Should().BeTrue();
            var cards = await service.GetAll("skalaja7");
            cards.Should().ContainSingle(card => card.Number == "12345");
        }

        [Fact]
        public async Task AddDuplicateCard()
        {
            // Arrange
            var di = Services.BuildServiceProvider();
            var service = di.GetRequiredService<ICardService>();

            // Act
            bool result1 = await InsertCard(service, "123456");
            bool result2 = await InsertCard(service, "123456");

            // Assert
            result1.Should().BeTrue();
            result2.Should().BeFalse();
        }

        [Fact]
        public async Task Authenticate()
        {
            // Arrange
            var di = Services.BuildServiceProvider();
            var service = di.GetRequiredService<ICardService>();
            var authenticator = di.GetRequiredService<ICardAuthenticator>();

            // Act
            bool result = await InsertCard(service, "1234567");
            var wrong = await authenticator.Authenticate("1234567", "4321");
            var username = await authenticator.Authenticate("1234567", "1234");

            // Assert
            result.Should().BeTrue();
            wrong.Should().BeNullOrEmpty();
            username.Should().Be("skalaja7");
        }

        [Fact]
        public async Task AuthenticateWithToken()
        {
            // Arrange
            var di = Services.BuildServiceProvider();
            var service = di.GetRequiredService<ICardService>();
            var authenticator = di.GetRequiredService<ICardAuthenticator>();

            // Act
            bool result = await InsertCard(service, "1337");
            var wrong = await authenticator.GetToken("1337", "4321");
            var token = await authenticator.GetToken("1337", "1234");

            // Assert
            result.Should().BeTrue();
            wrong.Should().BeNullOrEmpty();
            token.Should().NotBeNullOrEmpty();

            var username = await authenticator.AuthenticateWithToken("1337", token);
            username.Should().Be("skalaja7");
        }

        [Fact]
        public async Task Locking()
        {

        }
    }
}
