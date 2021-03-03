using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SelfServiceLibrary.API.Options;
using SelfServiceLibrary.Service.Interfaces;

namespace SelfServiceLibrary.API.Handlers
{
    public class CvutAuthHandler : AuthenticationHandler<CvutAuthOptions>
    {
        private readonly ICardAuthenticator _authenticator;

        public CvutAuthHandler(
            IOptionsMonitor<CvutAuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ICardAuthenticator authenticator) : base(options, logger, encoder, clock)
        {
            _authenticator = authenticator;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var basic = Request
                .Headers["Authorization"]
                .FirstOrDefault()?
                .Split(" ")
                .Last() ?? string.Empty;

            if (string.IsNullOrEmpty(basic))
                return AuthenticateResult.Fail($"Empty card number and or pin.");

            var credentialString = Encoding.UTF8.GetString(Convert.FromBase64String(basic));
            var credentials = credentialString.Split(':');
            var card = credentials.FirstOrDefault() ?? string.Empty;
            var pin = credentials.LastOrDefault();

            var username = await _authenticator.Authenticate(card, pin);

            if (string.IsNullOrEmpty(username))
                return AuthenticateResult.Fail("Wrong card number or pin");

            var claims = new[] { new Claim(ClaimTypes.Name, username) };

            var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
            var identities = new List<ClaimsIdentity> { identity };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, Options.Scheme);

            return AuthenticateResult.Success(ticket);
        }
    }
}
