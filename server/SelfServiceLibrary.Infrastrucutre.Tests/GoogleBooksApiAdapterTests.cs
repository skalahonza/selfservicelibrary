
using System.Threading.Tasks;

using FluentAssertions;

using Google.Books.API;

using Microsoft.Extensions.DependencyInjection;

using SelfServiceLibrary.BL.DTO.Book;

using Xunit;

namespace SelfServiceLibrary.Infrastrucutre.Tests
{
    public class GoogleBooksApiAdapterTests
    {
        private readonly GoogleBooksApiAdapter _adapter;

        public GoogleBooksApiAdapterTests()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            services.AddHttpClient<GoogleBooksApiAdapter>();
            var provider = services.BuildServiceProvider();
            _adapter = provider.GetRequiredService<GoogleBooksApiAdapter>();
        }

        [Theory]
        [InlineData("978-80-01-03775-1", "Úvod do algebry, zejména lineární", "Petr Olšák", 2007)]
        public async Task FillData(string isbn, string name, string author, int year)
        {
            // Arrange
            var book = new BookDetailDTO { ISBNorISSN = isbn };

            // Act
            var result = await _adapter.FillData(book);

            // Assert
            result.Should().BeTrue();
            book.Name.Should().Be(name);
            book.Author.Should().Be(author);
            book.CoAuthors.Should().BeEmpty();
            book.YearOfPublication.Should().Be(year);
        }
    }
}
