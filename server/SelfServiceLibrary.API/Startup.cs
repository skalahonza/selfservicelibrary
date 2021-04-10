using System;
using System.IO;
using System.Reflection;

using AutoMapper;

using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json.Converters;

using SelfServiceLibrary.API.Extensions;
using SelfServiceLibrary.API.Options;
using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.BL.Services;
using SelfServiceLibrary.BL.Validation;
using SelfServiceLibrary.Card.Authentication.Extensions;
using SelfServiceLibrary.CSV;
using SelfServiceLibrary.DAL;
using SelfServiceLibrary.DAL.Extensions;
using SelfServiceLibrary.Mapping;
using SelfServiceLibrary.Mapping.Profiles;

namespace SelfServiceLibrary.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

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
                });

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "Please enter your card number and pin.",
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header,
                    Scheme = "basic",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Basic"
                    }
                };
                c.AddSecurityDefinition("Basic", securitySchema);
                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Basic" } }
                };
                c.AddSecurityRequirement(securityRequirement);

                var assemblies = new[] { Assembly.GetExecutingAssembly(), typeof(BookListDTO).Assembly };
                foreach (var assembly in assemblies)
                {
                    // Set the comments path for the Swagger JSON and UI.
                    var xmlFile = $"{assembly.GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                }
            });

            services.AddSwaggerGenNewtonsoftSupport();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<BookAddDTOValidator>())
                .AddNewtonsoftJson(x => x.SerializerSettings.Converters.Add(new StringEnumConverter()));
            ConfigureSwagger(services);

            // Business logic
            services.AddScoped<IBookService, BookService>();

            // CSV
            services.AddScoped<ICsvService, CsvHelperAdapter>();

            // Mapping
            services.AddAutoMapper(typeof(BookProfile));
            services.AddScoped<BL.Interfaces.IMapper, AutoMapperAdapter>();

            // Persistence, MongoDB
            services.AddMongoDbPersistence(Configuration.GetSection("MongoDb"));

            // Id cards
            services.AddCardAuthentication(Configuration.GetSection("Identity"));            

            // Auth
            services.AddOptions<CvutAuthOptions>().Bind(Configuration).ValidateDataAnnotations();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CvutAuthOptions.DefaultScheme;
                options.DefaultChallengeScheme = CvutAuthOptions.DefaultScheme;
            })
           .AddCVUT(_ => { });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MongoDbContext dbContext)
        {
            // database configuration, creating indexes, configuring primary keys
            dbContext.EnsureIndexesExist().Wait();

            ConfigureSwagger(app);

            app.UseRouting();
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
