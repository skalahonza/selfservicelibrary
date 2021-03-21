using System.Collections.Generic;

using SelfServiceLibrary.Domain.Enums;

namespace SelfServiceLibrary.Service.DTO.User
{
    public class UserListDTO
    {
        public string? Username { get; set; }
        public HashSet<Role> Roles { get; set; } = new HashSet<Role>();
    }
}
