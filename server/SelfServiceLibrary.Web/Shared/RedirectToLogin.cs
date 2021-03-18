using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace SelfServiceLibrary.Web.Shared
{
    public class RedirectToLogin : ComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var redirectUri = NavigationManager.Uri;
            if (!state.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo($"/login?redirectUri={redirectUri}", true);
            }
        }
    }
}