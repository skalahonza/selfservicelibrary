
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
using System.Linq;
using System.Security.Claims;
using Pathoschild.Http.Client;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;

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

                    // TODO might be extended
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);

                    options.Events = new CookieAuthenticationEvents
                    {
                        // After the auth cookie has been validated, this event is called.
                        // In it we see if the access token is close to expiring.  If it is
                        // then we use the refresh token to get a new access token and save them.
                        // If the refresh token does not work for some reason then we redirect to 
                        // the login screen.
                        OnValidatePrincipal = async context =>
                        {
                            var timeRemaining = context.Properties.ExpiresUtc - DateTimeOffset.UtcNow;

                            // TODO: Get this from configuration with a fall back value.
                            var refreshThresholdMinutes = 5;
                            var refreshThreshold = TimeSpan.FromMinutes(refreshThresholdMinutes);

                            if (timeRemaining < refreshThreshold)
                            {
                                var refreshToken = context.Properties.GetTokenValue("refresh_token");
                                try
                                {
                                    // refresh tokens 
                                    var zuul = context.HttpContext.RequestServices.GetRequiredService<ZuulClient>();
                                    var response = await zuul.Refresh(refreshToken);
                                    context.Properties.UpdateTokenValue("access_token", response.AccessToken);
                                    context.Properties.UpdateTokenValue("refresh_token", response.RefreshToken);

                                    // refresh USERMAP roles
                                    var usermap = context.HttpContext.RequestServices.GetRequiredService<UsermapClient>();
                                    var info = await zuul.CheckToken(response.AccessToken);
                                    var user = await usermap.Get(info.UserName, response.AccessToken);
                                    var claims = user
                                        .Roles
                                        .Concat(user.TechnicalRoles)
                                        .Select(role => new Claim(ClaimTypes.Role, role))
                                        .Append(new Claim(ClaimTypes.Name, user.Username))
                                        .Append(new Claim(ClaimTypes.Email, user.PreferredEmail))
                                        .Append(new Claim(ClaimTypes.GivenName, user.FirstName))
                                        .Append(new Claim(ClaimTypes.Surname, user.LastName));

                                    var claimsToRefresh = new HashSet<string>
                                    {
                                        ClaimTypes.Role,
                                        ClaimTypes.Name,
                                        ClaimTypes.Email,
                                        ClaimTypes.GivenName,
                                        ClaimTypes.Surname
                                    };

                                    var identity = context.Principal.Identities.FirstOrDefault(x => x.AuthenticationType == "CVUT");
                                    foreach (var claim in identity.Claims.Where(x => claimsToRefresh.Contains(x.Type)).ToArray())
                                    {
                                        identity.RemoveClaim(claim);
                                    }
                                    identity.AddClaims(claims);
                                    context.ShouldRenew = true;
                                }
                                catch (ApiException)
                                {
                                    context.RejectPrincipal();
                                    await context.HttpContext.SignOutAsync();
                                }
                            }
                        }
                    };

                    options.Validate();
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
                    options.SaveTokens = true;

                    // fetch user context
                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var zuul = context.HttpContext.RequestServices.GetRequiredService<ZuulClient>();
                            var usermap = context.HttpContext.RequestServices.GetRequiredService<UsermapClient>();

                            var info = await zuul.CheckToken(context.AccessToken);
                            var user = await usermap.Get(info.UserName, context.AccessToken);

                            // claims mapping
                            var claims = user
                                .Roles
                                .Concat(user.TechnicalRoles)
                                .Select(role => new Claim(ClaimTypes.Role, role))
                                .Append(new Claim(ClaimTypes.Name, user.Username))
                                .Append(new Claim(ClaimTypes.Email, user.PreferredEmail))
                                .Append(new Claim(ClaimTypes.GivenName, user.FirstName))
                                .Append(new Claim(ClaimTypes.Surname, user.LastName));

                            context.Identity.AddClaims(claims);

                            // TODO should be set from token info
                            context.Properties.ExpiresUtc = DateTime.UtcNow.AddMinutes(59);
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
