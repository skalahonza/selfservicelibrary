using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SelfServiceLibrary.API.Interfaces;
using SelfServiceLibrary.API.Options;
using SelfServiceLibrary.BL.Interfaces;

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
        private readonly IUserContextService _userContextService;

        public CvutAuthHandler(
            IOptionsMonitor<CvutAuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock, ITokenService tokenService, IUserContextService userContextService) : base(options, logger, encoder, clock)
        {
            _tokenService = tokenService;
            _userContextService = userContextService;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var token = await _tokenService.CheckToken(Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last() ?? string.Empty);
            if (!token.IsValid)
                return AuthenticateResult.Fail($"{token.Error} {token.ErrorDescription}.");
            var context = await _userContextService.GetInfo(token.UserName ?? string.Empty);

            var claims = context
                .Roles
                .Concat(context.TechnicalRoles)
                .Select(role => new Claim(ClaimTypes.Role, role))
                .Append(new Claim(ClaimTypes.Name, context.Username))
                .Append(new Claim(ClaimTypes.GivenName, context.FirstName))
                .Append(new Claim(ClaimTypes.Surname, context.LastName));

            var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
            var identities = new List<ClaimsIdentity> { identity };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, Options.Scheme);

            return AuthenticateResult.Success(ticket);
        }
    }
}
