
using Newtonsoft.Json;

namespace Google.Books.API.Model
{
    public class Epub
    {
        [JsonProperty("isAvailable")]
        public bool IsAvailable { get; set; }
    }
}
