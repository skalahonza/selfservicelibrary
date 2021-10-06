using Microsoft.Extensions.Configuration;

namespace SelfServiceLibrary.Web.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetBasePath(this IConfiguration configuration) =>
            configuration["ASPNETCORE_BASEPATH"];
    }
}
