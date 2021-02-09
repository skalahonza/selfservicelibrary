using Microsoft.AspNetCore.Authentication;

namespace SelfServiceLibrary.API.Options
{
    public class CvutAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "Bearer";
        public string Scheme => DefaultScheme;
        public string AuthenticationType = DefaultScheme;
    }
}
