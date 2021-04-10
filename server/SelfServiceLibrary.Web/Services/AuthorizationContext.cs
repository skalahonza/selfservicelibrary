using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.Authorization;

using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.DAL.Enums;
using SelfServiceLibrary.Web.Extensions;

namespace SelfServiceLibrary.Web.Services
{
    public class AuthorizationContext : IAuthorizationContext
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthorizationContext(AuthenticationStateProvider authenticationStateProvider) => 
            _authenticationStateProvider = authenticationStateProvider;

        public async Task<bool> CanBorrow()
        {
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();

            if(state.User.Claims.GetRoles().Contains(Role.Librarian))
            {
                return true;
            }

            // KIOSK
            if(state.User.HasClaim("KIOSK", "KIOSK"))
            {
                return state.User.Claims.GetRoles().Contains(Role.KioskUser);
            }

            // Online
            else
            {
                return state.User.Claims.GetRoles().Contains(Role.SelfServiceUser);
            }
        }

        public async Task<bool> CanReturnFor()
        {
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return state.User.Claims.GetRoles().Contains(Role.Librarian);
        }

        public async Task<bool> CanBorrowTo()
        {
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return state.User.Claims.GetRoles().Contains(Role.Librarian);
        }

        public async Task<bool> CanGrantSelfService()
        {
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return state.User.Claims.GetRoles().Contains(Role.Librarian);
        }

        public async Task<bool> CanManageBooks()
        {
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return state.User.Claims.GetRoles().Contains(Role.Librarian);
        }

        public async Task<bool> CanManageLibrarians()
        {
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();            
            return state.User.Claims.GetRoles().Contains(Role.Admin);
        }

        public async Task<UserInfoDTO> GetUserInfo()
        {
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return state.User.Claims.GetUserBasicInfo();
        }
    }
}
