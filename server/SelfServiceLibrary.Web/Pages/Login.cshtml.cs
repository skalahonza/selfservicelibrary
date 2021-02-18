using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SelfServiceLibrary.Web.Pages
{
    [IgnoreAntiforgeryToken]
    public class LoginModel : PageModel
    {
        public async Task OnGet()
        {
            await HttpContext.ChallengeAsync("CVUT", new AuthenticationProperties
            {
            });
        }
    }
}