using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace SelfServiceLibrary.Web.Shared
{
    public class RedirectToLogin : ComponentBase
    {
#pragma warning disable CS8618 // Enforced by INJECT
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }
#pragma warning restore CS8618 // Enforced by INJECT

        protected override async Task OnInitializedAsync()
        {
            var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var redirectUri = NavigationManager.Uri;
            if (!(state.User.Identity?.IsAuthenticated ?? false))
            {
                NavigationManager.NavigateTo($"/login?redirectUri={redirectUri}", true);
            }
        }
    }
}