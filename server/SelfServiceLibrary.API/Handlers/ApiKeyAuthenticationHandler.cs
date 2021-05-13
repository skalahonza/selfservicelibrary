using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SelfServiceLibrary.API.Options;

namespace SelfServiceLibrary.API.Handlers
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        public const string ApiKeyHeaderName = "X-Api-Key";

        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
            : base(options, logger, encoder, clock)
        {
        }

        private AuthenticationTicket MakeTicket()
        {
            var identity = new ClaimsIdentity(Options.AuthenticationType);
            var identities = new List<ClaimsIdentity> { identity };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, Options.Scheme);
            return ticket;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var validKeys = OptionsMonitor.CurrentValue.ApiKeys ?? Array.Empty<string>();

            // no API key configured
            if (validKeys.Length == 0 || validKeys.All(string.IsNullOrEmpty))
            {
                return Task.FromResult(AuthenticateResult.Success(MakeTicket()));
            }

            // no API key in headers
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
            {
                return Task.FromResult(AuthenticateResult.Fail($"Missing {ApiKeyHeaderName} header."));
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if (apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKey))
            {
                return Task.FromResult(AuthenticateResult.Fail($"Missing {ApiKeyHeaderName} header."));
            }

            // API Key valid
            if (validKeys.Contains(providedApiKey))
            {
                return Task.FromResult(AuthenticateResult.Success(MakeTicket()));
            }
            // API Key not valid
            else
            {
                return Task.FromResult(AuthenticateResult.Fail($"Invalid API key."));
            }
        }
    }
}
