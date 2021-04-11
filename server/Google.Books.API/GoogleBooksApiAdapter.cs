using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Google.Books.API.Model;

using Microsoft.Extensions.Caching.Memory;

using Pathoschild.Http.Client;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.Interfaces;

namespace Google.Books.API
{
    public class GoogleBooksApiAdapter : IBookLookupService
    {
        private readonly IClient _client;
        private readonly IMemoryCache _cache;

        public GoogleBooksApiAdapter(HttpClient client, IMemoryCache cache)
        {
            _client = new FluentClient(new Uri("https://www.googleapis.com"), client);
            _cache = cache;
        }

        public async Task<bool> FillData(BookDetailDTO book)
        {
            var isbn = new string(book.ISBNorISSN.Where(char.IsLetterOrDigit).ToArray());
            var response = await _cache.GetOrCreateAsync(isbn, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                return _client.GetAsync($"books/v1/volumes?q=isbn:{isbn}").As<GoogleBookResponse>();
            });

            if (response.Items.Count == 1)
            {
                var item = response.Items.First();
                book.Name = item.VolumeInfo.Title;
                book.Author = item.VolumeInfo.Authors.First();
                book.CoAuthors = item.VolumeInfo.Authors.Skip(1).ToList();
                book.Pages = item.VolumeInfo.PageCount;
                book.Keywords.AddRange(item.VolumeInfo.Categories.Where(x => !string.IsNullOrEmpty(x)));
                if(!string.IsNullOrEmpty(item.VolumeInfo.Publisher))
                    book.Publisher = item.VolumeInfo.Publisher;
                if (int.TryParse(item.VolumeInfo.PublishedDate, out var year))
                    book.YearOfPublication = year;
                return true;
            }
            return false;
        }
    }
}
