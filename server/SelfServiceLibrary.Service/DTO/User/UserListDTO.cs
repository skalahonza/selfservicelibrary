using System.Collections.Generic;

using SelfServiceLibrary.DAL.Enums;

namespace SelfServiceLibrary.BL.DTO.User
{
    public class UserListDTO
    {
        public string? Username { get; set; }
        public HashSet<Role> Roles { get; set; } = new HashSet<Role>();
    }
}
