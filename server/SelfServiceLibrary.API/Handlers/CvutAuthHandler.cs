using CVUT.Auth;
using CVUT.Usermap;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        private readonly ZuulClient _zuul;
        private readonly UsermapClient _userProvider;

        public CvutAuthHandler(
            IOptionsMonitor<CvutAuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ZuulClient zuul,
            UsermapClient userProvider)
            : base(options, logger, encoder, clock)
        {
            _zuul = zuul;
            _userProvider = userProvider;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string accessToken = Request
                            .Headers["Authorization"]
                            .FirstOrDefault()?
                            .Split(" ")
                            .Last() ?? string.Empty;
            var tokenInfo = await _zuul.CheckToken(accessToken);
            if (!tokenInfo.IsValid)
                return AuthenticateResult.Fail($"{tokenInfo.Error} {tokenInfo.ErrorDescription}.");

            if (string.IsNullOrEmpty(tokenInfo.UserName))
                return AuthenticateResult.Fail("Not a user access.");

            var user = await _userProvider.Get(tokenInfo.UserName, accessToken);
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
