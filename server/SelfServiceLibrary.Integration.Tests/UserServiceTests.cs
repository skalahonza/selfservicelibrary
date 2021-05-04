using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.DAL.Enums;
using SelfServiceLibrary.Integration.Tests.Helpers;

using Xunit;

namespace SelfServiceLibrary.Integration.Tests
{
    public class UserServiceTests : IntegrationTestBase, IClassFixture<DbFixture>
    {
        private readonly IUserService _userService;

        public UserServiceTests(DbFixture fixture) : base(fixture)
        {
            _userService = Services.BuildServiceProvider().GetRequiredService<IUserService>();
        }

        private async Task AssertIsInRole(string username, Role role)
        {
            (await _userService.IsInRole(username, role)).Should().BeTrue();
            var roles = await _userService.GetRoles(username);
            roles.Should().Contain(role);

            var users = await _userService.GetAll(role);
            users.Should().Contain(x => x.Username == username);

            users = await _userService.GetAll();
            users.Should().Contain(x => x.Username == username && x.Roles.Contains(role));
        }

        private async Task AssertIsNotInRole(string username, Role role)
        {
            (await _userService.IsInRole(username, role)).Should().BeFalse();
            var roles = await _userService.GetRoles(username);
            roles.Should().NotContain(role);

            var users = await _userService.GetAll(role);
            users.Should().NotContain(x => x.Username == username);

            users = await _userService.GetAll();
            users.Should().NotContain(x => x.Username == username && x.Roles.Contains(role));
        }

        [Theory]
        [InlineData("visitor", Role.Visitor)]
        [InlineData("KioskUser", Role.KioskUser)]
        [InlineData("SelfServiceUser", Role.SelfServiceUser)]
        [InlineData("Librarian", Role.Librarian)]
        public async Task AddRole(string username, Role role)
        {
            // Act
            await _userService.AddRole(username, role);

            // Assert
            await AssertIsInRole(username, role);
        }

        [Theory]
        [InlineData("visitor2", Role.Visitor)]
        [InlineData("KioskUser2", Role.KioskUser)]
        [InlineData("SelfServiceUser2", Role.SelfServiceUser)]
        [InlineData("Librarian2", Role.Librarian)]
        public async Task RemoveRole(string username, Role role)
        {
            // Arrange
            await _userService.AddRole(username, role);

            // Act
            await _userService.RemoveRole(username, role);

            // Assert
            await AssertIsNotInRole(username, role);
        }

        [Fact]
        public async Task UpdateInfo()
        {
            // Arrange
            string username = "skala_honza";
            await _userService.AddRole(username, Role.Visitor);

            // Act
            var info = new UserInfoDTO
            {
                Email = "skalaja7@fel.cvut.cz",
                FirstName = "Jan",
                LastName = "Skála",
                FullName = "Jan Skála"
            };
            await _userService.UpdateInfo(username, info);

            // Assert
            var users = await _userService.GetAll();
            users.Should().ContainSingle(x =>
                x.Username == username &&
                x.InfoEmail == info.Email &&
                x.InfoFullName == info.FullName
            );
        }
    }
}
