using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

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
    }
}
