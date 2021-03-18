
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using SelfServiceLibrary.Card.Authentication.Options;

namespace SelfServiceLibrary.Card.Authentication.Providers
{
    public class CardLoginTokenProvider<TUser> : DataProtectorTokenProvider<TUser>
        where TUser : class
    {
        public CardLoginTokenProvider(IDataProtectionProvider dataProtectionProvider, IOptions<CardLoginTokenProviderOptions> options)
            : base(dataProtectionProvider, options)
        {
        }
    }
}
