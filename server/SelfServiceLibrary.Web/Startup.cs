
using AutoMapper;

using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using MongoDB.Driver;

using SelfServiceLibrary.API.Services;
using SelfServiceLibrary.CSV;
using SelfServiceLibrary.Mapping;
using SelfServiceLibrary.Mapping.Profiles;
using SelfServiceLibrary.Persistence.Options;
using SelfServiceLibrary.Service.Interfaces;
using SelfServiceLibrary.Service.Services;
using SelfServiceLibrary.Service.Validation;

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
