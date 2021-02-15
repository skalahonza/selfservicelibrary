using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SelfServiceLibrary.API.Interfaces;
using SelfServiceLibrary.API.Options;
using SelfServiceLibrary.Service.Interfaces;

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SelfServiceLibrary.API.Handlers
{
    public class CvutAuthHandler : AuthenticationHandler<CvutAuthOptions>
    {
        private readonly ITokenService _tokenService;
        private readonly IUserProvider _userProvider;

        public CvutAuthHandler(
            IOptionsMonitor<CvutAuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ITokenService tokenService,
            IUserProvider userProvider)
            : base(options, logger, encoder, clock)
        {
            _tokenService = tokenService;
            _userProvider = userProvider;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var token = await _tokenService.CheckToken(Request
                .Headers["Authorization"]
                .FirstOrDefault()?
                .Split(" ")
                .Last() ?? string.Empty);
            if (!token.IsValid)
                return AuthenticateResult.Fail($"{token.Error} {token.ErrorDescription}.");

            if (string.IsNullOrEmpty(token.UserName))
                return AuthenticateResult.Fail("Not a user access.");

            var user = await _userProvider.Get(token.UserName);
            var claims = user
                .Roles
                .Concat(user.TechnicalRoles)
                .Select(role => new Claim(ClaimTypes.Role, role))
                .Append(new Claim(ClaimTypes.Name, user.Username))
                .Append(new Claim(ClaimTypes.GivenName, user.FirstName))
                .Append(new Claim(ClaimTypes.Surname, user.LastName));

            var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
            var identities = new List<ClaimsIdentity> { identity };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, Options.Scheme);

            return AuthenticateResult.Success(ticket);
        }
    }
}
