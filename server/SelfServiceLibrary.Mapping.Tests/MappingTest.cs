
using AutoMapper;

using Microsoft.Extensions.DependencyInjection;

using SelfServiceLibrary.Mapping.Profiles;

using Xunit;

namespace SelfServiceLibrary.Mapping.Tests
{
    public class MappingTest
    {
        [Fact]
        public void MappingShouldBeValid()
        {
            var mapper = new ServiceCollection()
                .AddAutoMapper(typeof(BookProfile))
                .BuildServiceProvider()
                .GetRequiredService<IMapper>();
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
