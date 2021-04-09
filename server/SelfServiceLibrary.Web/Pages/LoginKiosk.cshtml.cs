using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using SelfServiceLibrary.BL.Interfaces;

namespace SelfServiceLibrary.Web.Pages
{
    [IgnoreAntiforgeryToken]
    public class LoginKioskModel : PageModel
    {
        public async Task OnGet([FromQuery] string redirectUri, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(redirectUri))
            {
                redirectUri = "/";
            }

            // KIOSK login
            await HttpContext.ChallengeAsync("KIOSK", new AuthenticationProperties { RedirectUri = redirectUri });
        }
    }
}