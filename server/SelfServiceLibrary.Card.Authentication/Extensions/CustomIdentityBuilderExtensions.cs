
using Microsoft.AspNetCore.Identity;

using SelfServiceLibrary.Card.Authentication.Providers;

namespace SelfServiceLibrary.Card.Authentication.Extensions
{
    internal static class CustomIdentityBuilderExtensions
    {
        internal static IdentityBuilder AddCardLoginTokenProvider(this IdentityBuilder builder)
        {
            var provider = typeof(CardLoginTokenProvider);
            return builder.AddTokenProvider(CardLoginTokenProvider.NAME, provider);
        }
    }
}
