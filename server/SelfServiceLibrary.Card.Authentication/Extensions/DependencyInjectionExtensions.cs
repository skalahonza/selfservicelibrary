﻿
using System;

using AspNetCore.Identity.Mongo;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.Card.Authentication.Model;
using SelfServiceLibrary.Card.Authentication.Options;
using SelfServiceLibrary.Card.Authentication.Services;

namespace SelfServiceLibrary.Card.Authentication.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddCardAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentityMongoDbProvider<IdCard>(options =>
            {
                // Password settings.                
                options.Password.RequiredLength = 0;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = true;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            },
            mongo =>
            {
                configuration.Bind(mongo);
                mongo.MigrationCollection = "migrations";
                mongo.RolesCollection = "roles";
                mongo.UsersCollection = "cards";

                services.Configure<MongoDbDatabaseOptions>(options =>
                {
                    options.DatabaseName = MongoUrl.Create(mongo.ConnectionString).DatabaseName ?? "default";
                    options.CollectionName = mongo.UsersCollection;
                });
            })
            .AddCardLoginTokenProvider();
            services.AddScoped<ICardAuthenticator, AspNetCoreIdentityAuthenticator>();
            return services;
        }
    }
}
