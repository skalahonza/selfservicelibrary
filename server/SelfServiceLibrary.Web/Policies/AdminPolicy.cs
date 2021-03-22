
using Microsoft.AspNetCore.Authorization;

using SelfServiceLibrary.DAL.Enums;

namespace SelfServiceLibrary.Web.Policies
{
    public static class AdminPolicy
    {
        public const string NAME = "admin";

        public static void Build(AuthorizationPolicyBuilder policy)
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole(nameof(Role.Admin));
        }
    }
}
