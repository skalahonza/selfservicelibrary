using System;
using System.IO;
using System.Reflection;

using AutoMapper;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using MongoDB.Driver;

using SelfServiceLibrary.API.Extensions;
using SelfServiceLibrary.API.Interfaces;
using SelfServiceLibrary.API.Middlewares;
using SelfServiceLibrary.API.Options;
using SelfServiceLibrary.API.Services;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.BL.Mapping;

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
            services.AddSwaggerGenNewtonsoftSupport();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Self Service Library API",
                    Description = "Self service library for university departments.",
                });

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "Please enter your JWT token into field without Bearer prefix, e.g. 4b8afda3-e6d9-4109-aef5-d7d9993e1821",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "GUID",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", securitySchema);
                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                };
                c.AddSecurityRequirement(securityRequirement);


                var assemblies = new[] { Assembly.GetExecutingAssembly(), typeof(IUserContextService).Assembly };
                foreach (var assembly in assemblies)
                {
                    // Set the comments path for the Swagger JSON and UI.
                    var xmlFile = $"{assembly.GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                }
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
            services.AddAutoMapper(typeof(BookProfile));
            services
                .AddOptions<MongoDbOptions>()
                .Bind(Configuration.GetSection("MongoDb"))
                .ValidateDataAnnotations();
            services.AddSingleton<IMongoClient, MongoClient>(x =>
            {
                var options = x.GetRequiredService<IOptions<MongoDbOptions>>();
                return new MongoClient(options.Value.ConnectionString);
            });
            services.AddTransient<BookService>();
            services.AddTransient<IssueService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseErrorHandlingMiddleware();
            ConfigureSwagger(app);

            app.UseRouting();
            if (env.IsDevelopment())
                app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
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
