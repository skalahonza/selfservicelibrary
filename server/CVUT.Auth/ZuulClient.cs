using CVUT.Auth.Model;
using CVUT.Auth.Options;

using Microsoft.Extensions.Options;

using Pathoschild.Http.Client;

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CVUT.Auth
{
    public class ZuulClient
    {
        private readonly IClient _client;
        private readonly IOptions<oAuth2Options> _options;

        public ZuulClient(IOptions<oAuth2Options> options, HttpClient client)
        {
            _options = options;
            _client = new FluentClient(new Uri("https://auth.fit.cvut.cz"), client);
        }

        /// <summary>
        /// Check token status
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ValueTask<TokenResponse> CheckToken(string token) =>
            new ValueTask<TokenResponse>(_client
                .PostAsync("oauth/check_token")
                .WithBody(p => p.FormUrlEncoded(new { token }))
                .WithOptions(true)
                .As<TokenResponse>());

        /// <summary>
        /// Refresh access token with refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public Task<SignInResponse> Refresh(string refreshToken) =>
            _client.PostAsync("oauth/token")
                .WithBasicAuthentication(_options.Value.ClientId, _options.Value.ClientSecret)
                .WithBody(p => p.FormUrlEncoded(new
                {
                    grant_type = "refresh_token",
                    refresh_token = refreshToken
                }))
                .As<SignInResponse>();

        /// <summary>
        /// Get access and refresh token by authorization code grant
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<SignInResponse> GetToken(string code) =>
            _client.PostAsync("oauth/token")
                .WithBasicAuthentication(_options.Value.ClientId, _options.Value.ClientSecret)
                .WithBody(p => p.FormUrlEncoded(new
                {
                    grant_type = "authorization_code",
                    code,
                    redirect_uri = _options.Value.RedirectUri
                }))
                .As<SignInResponse>();

        /// <summary>
        /// Get access token using client credentials grant
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        public Task<SignInResponse> GetToken(string clientId, string clientSecret) =>
            _client.PostAsync("oauth/token")
                .WithBasicAuthentication(clientId, clientSecret)
                .WithBody(p => p.FormUrlEncoded(new
                {
                    grant_type = "client_credentials"
                }))
                .As<SignInResponse>();
    }
}
