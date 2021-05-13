using System;

using Microsoft.AspNetCore.Authentication;

using SelfServiceLibrary.API.Handlers;
using SelfServiceLibrary.API.Options;

namespace SelfServiceLibrary.API.Extensions
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddApiKeySupport(this AuthenticationBuilder authenticationBuilder) =>
            authenticationBuilder.AddApiKeySupport(_ => { });

        public static AuthenticationBuilder AddApiKeySupport(this AuthenticationBuilder authenticationBuilder, Action<ApiKeyAuthenticationOptions> options) =>
            authenticationBuilder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.DefaultScheme, options);
    }
}
