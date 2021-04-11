using System.Collections.Generic;

using Newtonsoft.Json;

namespace Google.Books.API.Model
{
    public class GoogleBookResponse
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("totalItems")]
        public int TotalItems { get; set; }

        [JsonProperty("items")]
        public List<Item> Items { get; set; } = new List<Item>();
    }
}
