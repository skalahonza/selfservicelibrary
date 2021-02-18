
using System.Net.Http;
using System.Net.Http.Headers;

using AutoMapper;

using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

using CVUT.Auth.Options;

using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using MongoDB.Driver;

using Newtonsoft.Json.Linq;

using SelfServiceLibrary.CSV;
using SelfServiceLibrary.Mapping;
using SelfServiceLibrary.Mapping.Profiles;
using SelfServiceLibrary.Persistence.Options;
using SelfServiceLibrary.Service.Interfaces;
using SelfServiceLibrary.Service.Services;
using SelfServiceLibrary.Service.Validation;
using System.Text;
using System;
using CVUT.Usermap;
using CVUT.Auth;

namespace SelfServiceLibrary.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<BookAddDTOValidator>());
            services.AddServerSideBlazor();

            // Blazorise
            services
                .AddBlazorise(options => options.ChangeTextOnKeyPress = true)
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            // CVUT Auth
            services.AddOptions<oAuth2Options>().Bind(Configuration.GetSection("oAuth2")).ValidateDataAnnotations();
            services.AddHttpClient<ZuulClient>();

            // Usermap
            services.AddHttpClient<UsermapClient>();

            // AUTH
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "CVUT";
                })
                .AddCookie(options =>
                {
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.IsEssential = true;
                })
                .AddOAuth("CVUT", options =>
                {
                    options.ClientId = Configuration["oAuth2:ClientId"];
                    options.ClientSecret = Configuration["oAuth2:ClientSecret"];
                    options.CallbackPath = "/sign-in";

                    options.AuthorizationEndpoint = "https://auth.fit.cvut.cz/oauth/authorize";
                    options.TokenEndpoint = "https://auth.fit.cvut.cz/oauth/token";
                    options.UserInformationEndpoint = "https://auth.fit.cvut.cz/oauth/check_token";

                    // Zuul auth server requires client credentials to be send along with authorization code
                    var client = new HttpClient();
                    var basic = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Configuration["oAuth2:ClientId"]}:{Configuration["oAuth2:ClientSecret"]}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basic);
                    options.Backchannel = client;

                    // TODO claims mapping

                    // fetch user context
                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var zuul = context.HttpContext.RequestServices.GetRequiredService<ZuulClient>();
                            var usermap = context.HttpContext.RequestServices.GetRequiredService<UsermapClient>();

                            var info = await zuul.CheckToken(context.AccessToken);
                            var user = await usermap.Get(info.UserName, context.AccessToken);

                            user.GetHashCode();


                            return;
                        }
                    };

                    options.Validate();
                });

            // Business logic
            services.AddScoped<BookService>();
            services.AddScoped<IssueService>();

            // Persistence, MongoDB
            services
                .AddOptions<MongoDbOptions>()
                .Bind(Configuration.GetSection("MongoDb"))
                .ValidateDataAnnotations();
            services.AddSingleton<IMongoClient, MongoClient>(x =>
            {
                var options = x.GetRequiredService<IOptions<MongoDbOptions>>();
                return new MongoClient(options.Value.ConnectionString);
            });

            // Mapping
            services.AddAutoMapper(typeof(BookProfile));
            services.AddScoped<Service.Interfaces.IMapper, AutoMapperAdapter>();

            // CSV
            services.AddScoped<ICsvImporter, CsvImporter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Blazorise
            app
                .ApplicationServices
                .UseBootstrapProviders()
                .UseFontAwesomeIcons();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
