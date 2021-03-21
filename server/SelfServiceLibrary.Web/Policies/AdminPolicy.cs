
using Microsoft.AspNetCore.Authorization;

namespace SelfServiceLibrary.Web.Policies
{
    public static class AdminPolicy
    {
        public const string NAME = "admin";

        public static void Build(AuthorizationPolicyBuilder policy)
        {
            policy.RequireAuthenticatedUser();
        }
    }
}
