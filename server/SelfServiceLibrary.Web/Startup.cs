
using System.Net.Http;
using System.Net.Http.Headers;

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

using MongoDB.Driver;

using SelfServiceLibrary.CSV;
using SelfServiceLibrary.Mapping;
using SelfServiceLibrary.Mapping.Profiles;
using System.Text;
using System;
using CVUT.Usermap;
using CVUT.Auth;
using System.Linq;
using System.Security.Claims;
using Pathoschild.Http.Client;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using SelfServiceLibrary.Card.Authentication.Extensions;
using SelfServiceLibrary.Card.Authentication.Services;
using FluentValidation;
using SelfServiceLibrary.Web.Policies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using SelfServiceLibrary.Web.Options;
using SelfServiceLibrary.DAL.Enums;
using SelfServiceLibrary.DAL.Extensions;
using SelfServiceLibrary.DAL;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.BL.Services;
using SelfServiceLibrary.BL.Validation;
using SelfServiceLibrary.Web.Extensions;

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

            // Business logic
            services.AddScoped<BookService>();
            services.AddScoped<BookStatusService>();
            services.AddScoped<IssueService>();
            services.AddScoped<ICardService, CardService>();
            services.Decorate<ICardService, AspNetCoreIdentityDecorator>();
            services.AddScoped<UserService>();
            services.AddScoped<GuestService>();

            // Persistence, MongoDB
            services.AddMongoDbPersistence(Configuration.GetSection("MongoDb"));

            // Mapping
            services.AddAutoMapper(typeof(BookProfile));
            services.AddScoped<IMapper, AutoMapperAdapter>();

            // CSV
            services.AddScoped<ICsvService, CsvHelperAdapter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MongoDbContext dbContext)
        {
            // database configuration, creating indexes, configuring primary keys
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
