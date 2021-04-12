using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.Email;

using SendGrid.Extensions.DependencyInjection;

namespace SelfServiceLibrary.Card.Authentication.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddSmtpRelay(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        public static IServiceCollection AddSendGridEmailClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSendGrid(options => configuration.Bind(options));
            services.AddScoped<INotificationService, SendGridNotificationServiceAdapter>();
            return services;
        }
    }
}
