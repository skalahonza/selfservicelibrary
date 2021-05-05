using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using SelfServiceLibrary.BL.Extensions;

using Xunit;

namespace SelfServiceLibrary.Unit.Tests
{
    public class IAsyncEnumerableExtensions
    {
        private async IAsyncEnumerable<int> Generator(int size)
        {
            for (int i = 0; i < size; i++)
            {
                yield return i;
            }
        }

        [Theory]
        [InlineData(100,10)]
        [InlineData(50,7)]
        public async Task Batchify(int size, int limit)
        {
            // Arrange
            var source = Generator(size);

            // Act
            var processed = new List<int>();
            await foreach(var batch in source.Batchify(limit))
            {
                batch.Length.Should().BeLessOrEqualTo(limit);
                processed.AddRange(batch);
            }

            // Assert
            var all = await source.ToListAsync();
            processed.Should().Equal(all);
        }
    }
}
