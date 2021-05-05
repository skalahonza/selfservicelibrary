
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using Moq;

using SelfServiceLibrary.Web.Attributes;
using SelfServiceLibrary.Web.Extensions;

using Xunit;

namespace SelfServiceLibrary.Unit.Tests
{
    public class TestComponent : ComponentBase
    {
        [QueryStringParameter]
        public int PageSize { get; set; } = 5;

        [QueryStringParameter]
        public string Type { get; set; }
    }

    public class MockNavigationManager : NavigationManager
    {
        public MockNavigationManager(string uri)
        {
            Initialize(uri, uri);
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            NotifyLocationChanged(false);
        }
    }

    public class QueryStringParameterExtensions
    {
        [Theory]
        [InlineData("https://localhost/books?Page=1&PageSize=10&Type=Report", 10, "Report")]
        [InlineData("https://localhost/books?Page=1&Type=Report", 5, "Report")]
        public void SetParametersFromQueryString(string url, int pageSize, string type)
        {
            // Arrange
            var component = new TestComponent();
            var manager = new MockNavigationManager(url);

            // Act
            component.SetParametersFromQueryString(manager);

            // Assert
            component.PageSize.Should().Be(pageSize);
            component.Type.Should().Be(type);
        }

        [Theory]
        [InlineData(10, "Report", "https://localhost/books?PageSize=10&Type=Report")]
        public async Task UpdateQueryString(int pageSize, string type, string url)
        {
            // Arrange
            var mock = new Mock<IJSRuntime>();
            mock.Setup(x => x.InvokeAsync<object>(It.IsAny<string>(), It.IsAny<object[]>()))
                .Returns((string identifier, object[] args) =>
                {
                    // Assert
                    identifier.Should().Be("window.history.replaceState");
                    args[2].ToString().Should().Be(url);
                    return new ValueTask<object>();
                });
            var component = new TestComponent();

            // Act
            component.PageSize = pageSize;
            component.Type = type;
            await component.UpdateQueryString(new MockNavigationManager("https://localhost/books"), mock.Object);

            // Assert
            mock.Verify(x => x.InvokeAsync<object>(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }
    }
}
