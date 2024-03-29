
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

using CVUT.Auth.Options;

using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SelfServiceLibrary.CSV;
using SelfServiceLibrary.Mapping;
using SelfServiceLibrary.Mapping.Profiles;
using CVUT.Usermap;
using CVUT.Auth;
using SelfServiceLibrary.Card.Authentication.Extensions;
using SelfServiceLibrary.Card.Authentication.Services;
using FluentValidation;
using SelfServiceLibrary.Web.Policies;
using Microsoft.AspNetCore.HttpOverrides;
using SelfServiceLibrary.Web.Options;
using SelfServiceLibrary.DAL.Extensions;
using SelfServiceLibrary.DAL;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.BL.Services;
using SelfServiceLibrary.BL.Validation;
using SelfServiceLibrary.Web.Extensions;
using SelfServiceLibrary.Web.Services;
using SelfServiceLibrary.Web.Interfaces;
using Google.Books.API;

namespace SelfServiceLibrary.Web
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Miscellaneous 
            services.AddMemoryCache();

            // Blazor
            services.AddRazorPages()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<BookAddDTOValidator>());
            services.AddServerSideBlazor();

            // Validation
            services.AddValidatorsFromAssemblyContaining<BookAddDTOValidator>();

            // Blazorise
            services
                .AddBlazoriseWithFluentValidation(options =>
                {
                    options.ChangeTextOnKeyPress = false;
                    options.DelayTextOnKeyPress = true;
                    options.DelayTextOnKeyPressInterval = 300;
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            // Allows the app to recognize that the proxy uses HTTPS schema
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.RequireHeaderSymmetry = false;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            // Id cards
            services.AddCardAuthentication(Configuration.GetSection("Identity"));

            // CVUT Auth
            services.AddOptions<oAuth2Options>().Bind(Configuration.GetSection("oAuth2")).ValidateDataAnnotations();
            services.AddHttpClient<ZuulClient>();

            // Usermap
            services.AddOptions<UsermapClientOptions>().Bind(Configuration.GetSection("usermap")).ValidateDataAnnotations();
            services.AddHttpClient<UsermapClient>();
            services.AddHttpClient<IUserContextService, UsermapClient>();

            // Authentication
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "CVUT";
                })
                .AddAuthenticationCVUT(Configuration);

            // Authorization
            services.AddOptions<AdminOptions>().Bind(Configuration).ValidateDataAnnotations();
            services.AddAuthorization(options =>
            {
                options.AddPolicy(LibrarianPolicy.NAME, LibrarianPolicy.Build);
                options.AddPolicy(AdminPolicy.NAME, AdminPolicy.Build);
            });
            services.AddScoped<IAuthorizationContext, AuthorizationContext>();

            // Kiosk one time password authentication
            services.AddOptions<TotpOptions>().Bind(Configuration.GetSection("kiosk")).ValidateDataAnnotations();
            services.AddScoped<IOneTimePasswordService, TimeBasedOneTimePasswordService>();

            // Business logic
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IBookStatusService, BookStatusService>();
            services.AddScoped<IIssueService, IssueService>();
            services.AddScoped<ICardService, CardService>();
            services.Decorate<ICardService, AspNetCoreIdentityDecorator>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGuestService, GuestService>();

            // Persistence, MongoDB
            services.AddMongoDbPersistence(Configuration.GetSection("MongoDb"));

            // Mapping
            services.AddAutoMapper(typeof(BookProfile));
            services.AddScoped<IMapper, AutoMapperAdapter>();

            // CSV
            services.AddScoped<ICsvService, CsvHelperAdapter>();

            // Google maps
            services.AddHttpClient<IBookLookupService, GoogleBooksApiAdapter>();

            // Email
            if (_env.IsDevelopment())
            {
                services.AddSendGridEmailClient(Configuration.GetSection("SendGrid"));
            }
            else
            {
                services.AddSendGridEmailClient(Configuration.GetSection("SendGrid"));
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MongoDbContext dbContext)
        {
            // database configuration, creating indexes
            dbContext.EnsureIndexesExist().Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseForwardedHeaders();

            var basePath = Configuration["ASPNETCORE_BASEPATH"];

            if (!string.IsNullOrEmpty(basePath))
            {
                // server sub folder auth middleware rewrites
                app.Use(async (context, next) =>
                {
                    var url = context.Request.Path.Value ?? string.Empty;

                    // Rewrite to subfolder
                    if (url.Contains("/sign-in"))
                    {
                        // rewrite and continue processing
                        context.Request.Path = basePath + context.Request.Path;
                    }

                    await next();
                });
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
