using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

using SelfServiceLibrary.Web.Extensions;
using SelfServiceLibrary.Web.Interfaces;

namespace SelfServiceLibrary.Web.Pages
{
    [IgnoreAntiforgeryToken]
    public class LoginKioskModel : PageModel
    {
        private readonly IOneTimePasswordService _otp;
        private readonly IConfiguration _configuration;

        public LoginKioskModel(IOneTimePasswordService otp, IConfiguration configuration)
        {
            _otp = otp;
            _configuration = configuration;
        }

        public async Task OnGet([FromQuery] string redirectUri, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(redirectUri))
            {
                redirectUri = "/";
                if (!string.IsNullOrEmpty(_configuration.GetBasePath()))
                {
                    redirectUri = _configuration.GetBasePath();
                }
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