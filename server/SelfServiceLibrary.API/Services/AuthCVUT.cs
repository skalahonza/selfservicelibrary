using Microsoft.Extensions.Options;

using Pathoschild.Http.Client;

using SelfServiceLibrary.API.DTO;
using SelfServiceLibrary.API.Interfaces;
using SelfServiceLibrary.API.Options;

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SelfServiceLibrary.API.Services
{
    public class AuthCVUT : ITokenService
    {
        private readonly IClient _client;
        private readonly IOptions<oAuth2Options> _options;

        public AuthCVUT(IOptions<oAuth2Options> options, IHttpClientFactory factory)
        {
            _options = options;
            _client = new FluentClient(new Uri("https://auth.fit.cvut.cz"), factory.CreateClient());
        }

        public ValueTask<TokenResponse> CheckToken(string token) =>
            new ValueTask<TokenResponse>(_client
                .PostAsync("oauth/check_token")
                .WithBody(p => p.FormUrlEncoded(new { token }))
                .WithOptions(true)
                .As<TokenResponse>());

        public Task<SignInResponse> GetToken(string code) =>
            _client.PostAsync("oauth/token")
                .WithBasicAuthentication(_options.Value.ClientId!, _options.Value.ClientSecret!)
                .WithBody(p => p.FormUrlEncoded(new
                {
                    grant_type = "authorization_code",
                    code,
                    redirect_uri = _options.Value.RedirectUri
                }))
                .As<SignInResponse>();
    }
}
