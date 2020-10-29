using Newtonsoft.Json;

using System.Collections.Generic;

namespace SelfServiceLibrary.API.DTO
{
    public class TokenResponse
    {
        [JsonProperty("aud")]
        public IReadOnlyList<string>? Aud { get; set; }

        [JsonProperty("exp")]
        public int? Exp { get; set; }

        [JsonProperty("user_name")]
        public string? UserName { get; set; }

        [JsonProperty("authorities")]
        public IReadOnlyList<string>? Authorities { get; set; }

        [JsonProperty("client_id")]
        public string? ClientId { get; set; }

        [JsonProperty("scope")]
        public IReadOnlyList<string>? Scope { get; set; }

        [JsonProperty("error")]
        public string? Error { get; set; }

        [JsonProperty("error_description")]
        public string? ErrorDescription { get; set; }

        public bool IsValid => string.IsNullOrEmpty(Error);
    }
}
