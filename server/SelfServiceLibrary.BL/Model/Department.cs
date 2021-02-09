using Newtonsoft.Json;

namespace SelfServiceLibrary.BL.Model
{
    public class Department
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("nameCs")]
        public string NameCs { get; set; }

        [JsonProperty("nameEn")]
        public string NameEn { get; set; }
    }
}
