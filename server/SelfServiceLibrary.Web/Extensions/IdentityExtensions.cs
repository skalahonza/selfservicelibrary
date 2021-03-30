using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using SelfServiceLibrary.DAL.Entities;
using SelfServiceLibrary.DAL.Enums;

namespace SelfServiceLibrary.Web.Extensions
{
    public static class IdentityExtensions
    {
        public static ISet<Role> GetRoles(this IEnumerable<Claim> claims) =>
            claims
                .Where(x => x.Type == ClaimTypes.Role && Enum.TryParse<Role>(x.Value, out _))
                .Select(x => Enum.Parse<Role>(x.Value))
                .ToHashSet();

        public static string? GetUserName(this IEnumerable<Claim> claims) =>
            claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

        public static string? GetFirstName(this IEnumerable<Claim> claims) =>
            claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;

        public static string? GetLastName(this IEnumerable<Claim> claims) =>
            claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;

        public static string? GetEmail(this IEnumerable<Claim> claims) =>
            claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        public static UserInfo GetUserBasicInfo(this IEnumerable<Claim> claims) =>
            new UserInfo
            {
                Username = claims.GetUserName(),
                FirstName = claims.GetFirstName(),
                LastName = claims.GetLastName(),
                Email = claims.GetEmail()
            };
    }
}
