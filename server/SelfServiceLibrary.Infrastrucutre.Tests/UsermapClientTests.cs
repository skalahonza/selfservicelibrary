using System.Threading.Tasks;

using CVUT.Auth;
using CVUT.Auth.Options;
using CVUT.Usermap;

using FluentAssertions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.Mapping;
using SelfServiceLibrary.Mapping.Profiles;

using Xunit;

namespace SelfServiceLibrary.Infrastrucutre.Tests
{
    public class UsermapClientTests
    {
        private readonly UsermapClient _client;

        public UsermapClientTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets<ZuulClientTests>()
                .Build();

            var services = new ServiceCollection();
            services.AddOptions<UsermapClientOptions>().Bind(configuration.GetSection("usermap"));
            services.AddHttpClient<ZuulClient>();
            services.AddHttpClient<UsermapClient>();

            services.AddAutoMapper(typeof(BookProfile));
            services.AddScoped<IMapper, AutoMapperAdapter>();

            var provider = services.BuildServiceProvider();
            _client = provider.GetRequiredService<UsermapClient>();
        }

        [Theory]
        [InlineData("skalaja7", "Jan", "Skála")]
        public async Task Get(string username, string firstName, string lastName)
        {
            // Act
            var user = await _client.Get(username);

            // Assert
            user.FirstName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
        }

        [Theory]
        [InlineData("skála jan", 10,"Jan", "Skála")]
        public async Task Suggest(string term, int limit, string firstName, string lastName)
        {
            // Act
            var suggestions = await _client.Suggest(term, limit);

            // Assert
            suggestions.Count.Should().BeLessOrEqualTo(limit);
            suggestions.Should().Contain(user =>
                user.FirstName == firstName
                && user.LastName == lastName
            );
        }
    }
}
