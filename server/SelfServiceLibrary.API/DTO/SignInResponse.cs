﻿using Newtonsoft.Json;

namespace SelfServiceLibrary.API.DTO
{
    public class SignInResponse
    {
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string? TokenType { get; set; }

        [JsonProperty("refresh_token")]
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Seconds to expire
        /// </summary>
        [JsonProperty("expires_in")]
        public int? ExpiresIn { get; set; }

        [JsonProperty("scope")]
        public string? Scope { get; set; }
    }
}
