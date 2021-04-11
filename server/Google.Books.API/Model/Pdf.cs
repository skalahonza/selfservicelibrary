
using Newtonsoft.Json;

namespace Google.Books.API.Model
{
    public class Pdf
    {
        [JsonProperty("isAvailable")]
        public bool IsAvailable { get; set; }
    }
}
