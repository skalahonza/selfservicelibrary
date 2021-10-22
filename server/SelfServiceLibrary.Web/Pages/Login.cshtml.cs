using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using CVUT.Usermap;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.DAL.Enums;
using SelfServiceLibrary.Web.Extensions;
using SelfServiceLibrary.Web.Options;

namespace SelfServiceLibrary.Web.Pages
{
    [IgnoreAntiforgeryToken]
    public class LoginModel : PageModel
    {
        private readonly ICardAuthenticator _authenticator;
        private readonly UsermapClient _usermap;
        private readonly IOptionsMonitor<AdminOptions> _adminOptions;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public LoginModel(ICardAuthenticator authenticator, UsermapClient usermap, IOptionsMonitor<AdminOptions> options, IUserService userService, IConfiguration configuration)
        {
            _authenticator = authenticator;
            _usermap = usermap;
            _adminOptions = options;
            _userService = userService;
            _configuration = configuration;
        }

        public async Task OnGet([FromQuery] string redirectUri, [FromQuery] string card, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(redirectUri))
            {
                redirectUri = "/";
                if (!string.IsNullOrEmpty(_configuration.GetBasePath()))
                {
                    redirectUri = _configuration.GetBasePath();
                }
            }

            // Card authentication
            if (!string.IsNullOrEmpty(card) && !string.IsNullOrEmpty(token))
            {
                var username = await _authenticator.AuthenticateWithToken(card, token);
                if (string.IsNullOrEmpty(username))
                {
                    await HttpContext.ForbidAsync();
                }
                else
                {
                    var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
                    var user = await _usermap.Get(username);

                    // claims mapping
                    claims.AddRange(user
                        .Roles
                        .Concat(user.TechnicalRoles)
                        .Select(role => new Claim(ClaimTypes.Role, role))
                        .Append(new Claim(ClaimTypes.Email, user.PreferredEmail ?? string.Empty))
                        .Append(new Claim(ClaimTypes.GivenName, user.FirstName ?? string.Empty))
                        .Append(new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty))
                        );

                    if (_adminOptions.CurrentValue.Admins.Contains(username))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, nameof(Role.Admin)));
                    }

                    // get app roles
                    var roles = await _userService.GetRoles(username);
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                    }

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var identities = new List<ClaimsIdentity> { identity };
                    var principal = new ClaimsPrincipal(identities);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    HttpContext.Response.Redirect(redirectUri);
                }
            }

            // CVUT login
            else
            {
                await HttpContext.ChallengeAsync("CVUT", new AuthenticationProperties { RedirectUri = redirectUri });
            }
        }
    }
}