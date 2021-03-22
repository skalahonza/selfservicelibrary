
using Microsoft.AspNetCore.Authorization;

using SelfServiceLibrary.DAL.Enums;

namespace SelfServiceLibrary.Web.Policies
{
    public static class LibrarianPolicy
    {
        public const string NAME = "librarian";

        public static void Build(AuthorizationPolicyBuilder policy)
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole(nameof(Role.Librarian));
        }
    }
}
