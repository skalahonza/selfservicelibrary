using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using SelfServiceLibrary.API.Extensions;
using SelfServiceLibrary.API.Interfaces;
using SelfServiceLibrary.API.Options;
using SelfServiceLibrary.API.Services;
using SelfServiceLibrary.BL.Interfaces;

namespace SelfServiceLibrary.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureSwagger(IServiceCollection services)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Self Service Library API",
                    Description = "Self service library for university departments.",
                    Contact = new OpenApiContact
                    {
                        Name = "Jan Skála",
                        Url = new Uri("https://janskala.cz"),
                    },
                });
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureSwagger(services);
            services.AddControllers().AddNewtonsoftJson();

            services.AddOptions<CvutAuthOptions>().Bind(Configuration).ValidateDataAnnotations();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CvutAuthOptions.DefaultScheme;
                options.DefaultChallengeScheme = CvutAuthOptions.DefaultScheme;
            })
            .AddCVUT(_ => { });

            services.AddOptions<oAuth2Options>().Bind(Configuration.GetSection("oAuth2")).ValidateDataAnnotations();
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddTransient<IUserContextService, UsermapCVUT>();
            services.AddSingleton<ITokenService, AuthCVUT>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ConfigureSwagger(app);
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        public void ConfigureSwagger(IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Self Service Library v1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
