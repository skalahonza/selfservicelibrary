using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using SelfServiceLibrary.Service.Interfaces;

namespace SelfServiceLibrary.Web.Pages
{
    [IgnoreAntiforgeryToken]
    public class LoginModel : PageModel
    {
        private readonly ICardAuthenticator _authenticator;

        public LoginModel(ICardAuthenticator authenticator) =>
            _authenticator = authenticator;

        public async Task OnGet([FromQuery] string redirectUri, [FromQuery] string card, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(redirectUri))
            {
                redirectUri = "/";
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
                    var claims = new[] { new Claim(ClaimTypes.Name, username) };
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