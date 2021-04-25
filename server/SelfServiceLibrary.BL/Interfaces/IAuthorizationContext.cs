using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.User;

namespace SelfServiceLibrary.BL.Interfaces
{
    public interface IAuthorizationContext
    {
        Task<bool> CanBorrow();
        Task<bool> CanBorrowTo();
        Task<bool> CanReturnFor();
        Task<bool> CanManageContent();
        Task<bool> CanGrantSelfService();
        Task<bool> CanManageLibrarians();
        Task<UserInfoDTO?> GetUserInfo();
    }
}
