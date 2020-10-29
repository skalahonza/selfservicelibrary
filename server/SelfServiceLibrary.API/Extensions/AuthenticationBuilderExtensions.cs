using Microsoft.AspNetCore.Authentication;

using SelfServiceLibrary.API.Handlers;
using SelfServiceLibrary.API.Options;

using System;

namespace SelfServiceLibrary.API.Extensions
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddCVUT(this AuthenticationBuilder authenticationBuilder, Action<CvutAuthOptions> options) =>
            authenticationBuilder.AddScheme<CvutAuthOptions, CvutAuthHandler>(CvutAuthOptions.DefaultScheme, options);
    }
}
