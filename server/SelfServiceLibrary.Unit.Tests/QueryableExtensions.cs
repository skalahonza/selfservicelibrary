using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using FluentAssertions;

using SelfServiceLibrary.BL.Extensions;
using SelfServiceLibrary.DAL.Entities;

using Xunit;

namespace SelfServiceLibrary.Unit.Tests
{
    public class QueryableExtensions
    {
        [Fact]
        public void Sort()
        {
            // Arrange
            var books = new List<Book>
            {
                new()
                {
                    Name = "A",
                    Author = "John"
                },
                new()
                {
                    Name = "B",
                    Author = "John"
                },
                new()
                {
                    Name = "C",
                    Author = "Jane"
                },
                new()
                {
                    Name = "D",
                    Author = "Jane"
                },
            };

            // Act
            var query = books.AsQueryable().Sort(new[] { ("Author", ListSortDirection.Ascending), ("Name", ListSortDirection.Descending) });
            var output = query.ToList();

            // Assert
            output.Should().Equal(
                books[3],
                books[2],
                books[1],
                books[0]
            );
        }
    }
}
