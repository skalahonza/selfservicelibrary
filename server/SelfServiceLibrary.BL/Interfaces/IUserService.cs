using System.Collections.Generic;
using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.DAL.Enums;

namespace SelfServiceLibrary.BL.Interfaces
{
    public interface IUserService
    {
        Task UpdateInfo(string username, UserInfoDTO info);
        Task<bool> AddRole(string username, Role role);
        Task<List<UserListDTO>> GetAll();
        Task<List<UserListDTO>> GetAll(Role role);
        Task<HashSet<Role>> GetRoles(string username);
        Task<bool> IsInRole(string username, Role role);
        Task<bool> RemoveRole(string username, Role role);
    }
}