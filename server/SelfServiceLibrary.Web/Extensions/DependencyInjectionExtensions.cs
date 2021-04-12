using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using CVUT.Auth;
using CVUT.Usermap;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Pathoschild.Http.Client;

using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.DAL.Enums;
using SelfServiceLibrary.Web.Options;

namespace SelfServiceLibrary.Web.Extensions
{
    public static class DependencyInjectionExtensions
    {
        private static async Task<List<Claim>> GetClaims(this IServiceProvider services, string accessToken)
        {
            var zuul = services.GetRequiredService<ZuulClient>();

            var info = await zuul.CheckToken(accessToken);

            // non user token
            if (string.IsNullOrEmpty(info.UserName))
            {
                return new List<Claim>();
            }

            var usermap = services.GetRequiredService<UsermapClient>();
            var adminOptions = services.GetRequiredService<IOptionsMonitor<AdminOptions>>();
            var userService = services.GetRequiredService<IUserService>();
            var mapper = services.GetRequiredService<IMapper>();

            var user = await usermap.Get(info.UserName, accessToken);

            // update user info cache
            await userService.UpdateInfo(info.UserName, mapper.Map<UserInfoDTO>(user));

            // user probably not found in Usermap
            if (string.IsNullOrEmpty(user.Username))
            {
                return new List<Claim>();
            }

            // claims mapping
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.PreferredEmail ?? string.Empty),
                new Claim(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
                new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty)
            };

            // TODO MAP USERMAP ROLES based on mapping table
            var usermapRoles = user.Roles.Concat(user.TechnicalRoles);

            if (adminOptions.CurrentValue.Admins.Contains(user.Username))
            {
                claims.Add(new Claim(ClaimTypes.Role, nameof(Role.Admin)));
            }

            // get app roles
            var roles = await userService.GetRoles(user.Username);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }

            return claims;
        }

        private static void ConfigureOptions(this OAuthOptions options, IConfiguration Configuration)
        {
            options.ClientId = Configuration["oAuth2:ClientId"];
            options.ClientSecret = Configuration["oAuth2:ClientSecret"];

            options.AuthorizationEndpoint = "https://auth.fit.cvut.cz/oauth/authorize";
            options.TokenEndpoint = "https://auth.fit.cvut.cz/oauth/token";
            options.UserInformationEndpoint = "https://auth.fit.cvut.cz/oauth/check_token";

            // Zuul auth server requires client credentials to be send along with authorization code
            var client = new HttpClient();
            var basic = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Configuration["oAuth2:ClientId"]}:{Configuration["oAuth2:ClientSecret"]}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basic);
            options.Backchannel = client;
            options.SaveTokens = true;
        }

        public static AuthenticationBuilder AddAuthenticationCVUT(this AuthenticationBuilder builder, IConfiguration Configuration)
        {
            // Cookie authentication - the authentication ticket is inside a cookie
            return builder.AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.IsEssential = true;

                // TODO might be extended
                options.ExpireTimeSpan = TimeSpan.FromMinutes(120);

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
                        const int refreshThresholdMinutes = 5;
                        var refreshThreshold = TimeSpan.FromMinutes(refreshThresholdMinutes);

                        if (timeRemaining < refreshThreshold)
                        {
                            var refreshToken = context.Properties.GetTokenValue("refresh_token");
                            try
                            {
                                // refresh tokens 
                                var zuul = context.HttpContext.RequestServices.GetRequiredService<ZuulClient>();
                                var response = await zuul.Refresh(refreshToken ?? string.Empty);
                                context.Properties.UpdateTokenValue("access_token", response.AccessToken!);
                                context.Properties.UpdateTokenValue("refresh_token", response.RefreshToken!);

                                // refresh USERMAP roles
                                var claims = await context.HttpContext.RequestServices.GetClaims(response.AccessToken!);

                                var claimsToRefresh = new HashSet<string>
                                    {
                                        ClaimTypes.Role,
                                        ClaimTypes.Name,
                                        ClaimTypes.Email,
                                        ClaimTypes.GivenName,
                                        ClaimTypes.Surname
                                    };

                                var identity = context.Principal?.Identities.FirstOrDefault(x => x.AuthenticationType == "CVUT" || x.AuthenticationType == "KIOSK");
                                if (identity != null)
                                {
                                    foreach (var claim in identity.Claims.Where(x => claimsToRefresh.Contains(x.Type)).ToArray())
                                    {
                                        identity.RemoveClaim(claim);
                                    }
                                    identity.AddClaims(claims);
                                }
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
            // Zuul OAuth2
            .AddOAuth("CVUT", options =>
            {
                options.ConfigureOptions(Configuration);
                options.CallbackPath = "/sign-in";

                // fetch user context
                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var claims = await context.HttpContext.RequestServices.GetClaims(context.AccessToken);
                        context.Identity.AddClaims(claims);

                        // TODO should be set from token info
                        context.Properties.ExpiresUtc = DateTime.UtcNow.AddMinutes(59);
                    }
                };

                options.Validate();
            })

            // Zuul OAuth2 - kiosk schema
            .AddOAuth("KIOSK", options =>
            {
                options.ConfigureOptions(Configuration);
                options.CallbackPath = "/sign-in-kiosk";

                // fetch user context
                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var claims = await context.HttpContext.RequestServices.GetClaims(context.AccessToken);
                        // add KIOSK claim signalizing that the user authenticated through kiosk
                        claims.Add(new Claim("KIOSK", "KIOSK"));
                        context.Identity.AddClaims(claims);

                        // TODO should be set from token info
                        context.Properties.ExpiresUtc = DateTime.UtcNow.AddMinutes(59);
                    }
                };

                options.Validate();
            });
        }
    }
}
