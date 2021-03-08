
using Microsoft.AspNetCore.Authorization;

namespace SelfServiceLibrary.Web.Policies
{
    public static class LibrarianPolicy
    {
        public const string NAME = "librarian";

        public static void Build(AuthorizationPolicyBuilder policy)
        {
            policy.RequireAuthenticatedUser();
        }
    }
}
