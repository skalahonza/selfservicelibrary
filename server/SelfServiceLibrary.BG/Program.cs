using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SelfServiceLibrary.BG.Services;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.BL.Services;
using SelfServiceLibrary.Card.Authentication.Extensions;
using SelfServiceLibrary.CSV;
using SelfServiceLibrary.DAL.Extensions;
using SelfServiceLibrary.Mapping;
using SelfServiceLibrary.Mapping.Profiles;

namespace SelfServiceLibrary.BG
{
    public class Program
    {
        public static void Main(string[] args) => 
            CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var Configuration = hostContext.Configuration;

                    services.AddHostedService<Worker>();

                    // Authorization
                    services.AddSingleton<IAuthorizationContext, AuthorizationContext>();

                    // Persistence, MongoDB
                    services.AddMongoDbPersistence(Configuration.GetSection("MongoDb"));

                    // Mapping
                    services.AddAutoMapper(typeof(BookProfile));
                    services.AddScoped<IMapper, AutoMapperAdapter>();

                    // Email
                    if (hostContext.HostingEnvironment.IsDevelopment())
                    {
                        services.AddSendGridEmailClient(Configuration.GetSection("SendGrid"));
                    }
                    else
                    {
                        services.AddSendGridEmailClient(Configuration.GetSection("SendGrid"));
                    }
                });
    }
}
