﻿using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SelfServiceLibrary.Web.Pages
{
    [IgnoreAntiforgeryToken]
    public class LoginModel : PageModel
    {
        public Task OnGet()
        {
            return HttpContext.ChallengeAsync("CVUT", new AuthenticationProperties { RedirectUri = "/" });
        }
    }
}