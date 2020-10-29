using Microsoft.AspNetCore.Authentication;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SelfServiceLibrary.API.Options
{
    public class CvutAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "Bearer";
        public string Scheme => DefaultScheme;
        public string AuthenticationType = DefaultScheme;
    }
}
