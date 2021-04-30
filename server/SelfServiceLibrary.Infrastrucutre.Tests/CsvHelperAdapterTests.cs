
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.CSV;
using SelfServiceLibrary.Mapping;
using SelfServiceLibrary.Mapping.Profiles;

using Xunit;

namespace SelfServiceLibrary.Infrastrucutre.Tests
{
    public class CsvHelperAdapterTests
    {
        private readonly CsvHelperAdapter _adapter;

        public CsvHelperAdapterTests()
        {
            var services = new ServiceCollection();
            services.AddScoped<CsvHelperAdapter>();
            services.AddLogging();

            services.AddAutoMapper(typeof(BookProfile));
            services.AddScoped<IMapper, AutoMapperAdapter>();

            var provider = services.BuildServiceProvider();
            _adapter = provider.GetRequiredService<CsvHelperAdapter>();
        }

        [Theory]
        [InlineData("CSV/import.csv",9)]
        public async Task ImportCSV(string file, int count)
        {
            // Act
            var books = await _adapter.ImportBooks(File.OpenRead(file)).ToListAsync();

            // Assert
            books.Count.Should().Be(count);
        }

        [Theory]
        [InlineData("CSV/export.csv")]
        public async Task ExportCSV(string file)
        {
            // Arrange
            var stream = new MemoryStream();
            var books = _adapter.ImportBooks(File.OpenRead("CSV/import.csv"));

            // Act
            await _adapter.ExportBooks(books, stream, true);

            // Assert
            stream.Position = 0;
            var expected = await File.ReadAllTextAsync(file);
            var reader = new StreamReader(stream);
            var actual = reader.ReadToEnd();

            actual.Should().Be(expected);
        }
    }
}
