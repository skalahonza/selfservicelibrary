
using Newtonsoft.Json;

namespace Google.Books.API.Model
{
    public class ReadingModes
    {
        [JsonProperty("text")]
        public bool Text { get; set; }

        [JsonProperty("image")]
        public bool Image { get; set; }
    }
}
