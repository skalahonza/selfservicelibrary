
using Microsoft.AspNetCore.Authentication;

namespace SelfServiceLibrary.API.Options
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "API Key";
        public string Scheme => DefaultScheme;
        public string AuthenticationType = DefaultScheme;

        public string[]? ApiKeys { get; set; }
    }
}
