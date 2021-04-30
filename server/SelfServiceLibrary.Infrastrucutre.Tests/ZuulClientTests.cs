using System.Threading.Tasks;

using CVUT.Auth;
using CVUT.Auth.Options;

using FluentAssertions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Xunit;

namespace SelfServiceLibrary.Infrastrucutre.Tests
{
    public class ZuulClientTests
    {
        private readonly oAuth2Options _options;
        private readonly ZuulClient _client;

        public ZuulClientTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets<ZuulClientTests>()
                .Build();

            var services = new ServiceCollection();
            // use usermap config to test client credentials flow
            services.AddOptions<oAuth2Options>().Bind(configuration.GetSection("usermap"));
            services.AddHttpClient<ZuulClient>();

            var provider = services.BuildServiceProvider();
            _options = provider.GetRequiredService<IOptions<oAuth2Options>>().Value;
            _client = provider.GetRequiredService<ZuulClient>();
        }

        [Fact]
        public async Task GetToken()
        {
            // Act
            var response = await _client.GetToken(_options.ClientId, _options.ClientSecret);

            // Assert
            response.AccessToken.Should().NotBeNullOrEmpty(because: "Valid credentials were used.");
        }

        [Fact]
        public async Task CheckToken()
        {
            // Arrange
            var response = await _client.GetToken(_options.ClientId, _options.ClientSecret);

            // Act
            var check = await _client.CheckToken(response.AccessToken);

            // Assert
            check.IsValid.Should().BeTrue();
            check.Error.Should().BeNullOrEmpty();
            check.ErrorDescription.Should().BeNullOrEmpty();
        }
    }
}
