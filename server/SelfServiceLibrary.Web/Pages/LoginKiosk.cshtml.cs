using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using SelfServiceLibrary.Web.Interfaces;

namespace SelfServiceLibrary.Web.Pages
{
    [IgnoreAntiforgeryToken]
    public class LoginKioskModel : PageModel
    {
        private readonly IOneTimePasswordService _otp;

        public LoginKioskModel(IOneTimePasswordService otp) =>
            _otp = otp;

        public async Task OnGet([FromQuery] string redirectUri, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(redirectUri))
            {
                redirectUri = "/";
            }

            if (_otp.Verify(token))
            {
                // KIOSK login
                await HttpContext.ChallengeAsync("KIOSK", new AuthenticationProperties { RedirectUri = redirectUri });
            }
            else
            {
                await HttpContext.ForbidAsync();
            }
        }
    }
}