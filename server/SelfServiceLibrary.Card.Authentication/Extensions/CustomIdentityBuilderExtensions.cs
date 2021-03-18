
using Microsoft.AspNetCore.Identity;

using SelfServiceLibrary.Card.Authentication.Providers;

namespace SelfServiceLibrary.Card.Authentication.Extensions
{
    public static class CustomIdentityBuilderExtensions
    {
        public static IdentityBuilder AddCardLoginTokenProvider(this IdentityBuilder builder)
        {
            var userType = builder.UserType;
            var provider = typeof(CardLoginTokenProvider<>).MakeGenericType(userType);
            return builder.AddTokenProvider(CardLoginTokenProvider.NAME, provider);
        }
    }
}
