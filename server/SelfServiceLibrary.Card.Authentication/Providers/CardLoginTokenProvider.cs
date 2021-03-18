
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

namespace SelfServiceLibrary.Card.Authentication.Providers
{
    public class CardLoginTokenProvider<TUser> : TotpSecurityStampBasedTokenProvider<TUser>
        where TUser : class
    {
        public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user) =>
            Task.FromResult(false);
    }
}
