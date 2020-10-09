using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Pathoschild.Http.Client;

using SelfServiceLibrary.API.DTO;
using SelfServiceLibrary.API.Options;

namespace SelfServiceLibrary.API.Controllers
{

    public class AuthController : BaseController
    {
        private readonly IClient _client;
        private readonly IOptions<oAuth2Options> _options;

        public AuthController(IOptions<oAuth2Options> options)
        {
            _client = new FluentClient("https://auth.fit.cvut.cz");
            _options = options;
        }

        // sign in with code
        [HttpPost]
        public Task<SignInResponse> Login([FromBody] SignIn dto) =>
            _client
                .PostAsync("oauth/token")
                .WithBasicAuthentication(_options.Value.ClientId!, _options.Value.ClientSecret!)
                .WithBody(p => p.FormUrlEncoded(new
                {
                    grant_type = "authorization_code",
                    code = dto.Code,
                    redirect_uri = _options.Value.RedirectUri
                }))
                .As<SignInResponse>();
    }
}
