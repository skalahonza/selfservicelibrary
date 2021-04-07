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
            if (roles.Contains(Role.SelfServiceUser))
            {
                CurrentRole = Role.SelfServiceUser;
            }
            else if (roles.Contains(Role.KioskUser))
            {
                CurrentRole = Role.KioskUser;
            }
            else
            {
                CurrentRole = Role.Visitor;
            }
        }

        public string Username { get; set; } = string.Empty;

        public Role CurrentRole { get; set; }
    }
}
