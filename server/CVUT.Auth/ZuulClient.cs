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

        public ValueTask<TokenResponse> CheckToken(string token) =>
            new ValueTask<TokenResponse>(_client
                .PostAsync("oauth/check_token")
                .WithBody(p => p.FormUrlEncoded(new { token }))
                .WithOptions(true)
                .As<TokenResponse>());

        public Task<SignInResponse> Refresh(string refreshToken) =>
            _client.PostAsync("oauth/token")
                .WithBasicAuthentication(_options.Value.ClientId, _options.Value.ClientSecret)
                .WithBody(p => p.FormUrlEncoded(new
                {
                    grant_type = "refresh_token",
                    refresh_token = refreshToken
                }))
                .As<SignInResponse>();

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
    }
}
