using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SelfServiceLibrary.Web.Pages
{
    [IgnoreAntiforgeryToken]
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            //await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
            //await HttpContext.SignOutAsync("CVUT");
            /*
            await HttpContext.SignOutAsync("CVUT", new AuthenticationProperties
            {
                RedirectUri = "/",
            });
            */
        }
    }
}