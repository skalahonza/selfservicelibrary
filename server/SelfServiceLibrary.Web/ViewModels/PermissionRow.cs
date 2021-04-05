using System.Collections.Generic;

using SelfServiceLibrary.DAL.Enums;

namespace SelfServiceLibrary.Web.ViewModels
{
    public class PermissionRow
    {
        public PermissionRow() { }

        public PermissionRow(string? username, ISet<Role> roles)
        {
            Username = username ?? string.Empty;
            IsVisitor = roles.Contains(Role.Visitor);
            IsKioskUser = roles.Contains(Role.KioskUser);
            IsSelfServiceUser = roles.Contains(Role.SelfServiceUser);
        }

        public string Username { get; set; } = string.Empty;
        public bool IsVisitor { get; set; }
        public bool IsKioskUser { get; set; }
        public bool IsSelfServiceUser { get; set; }
    }
}
