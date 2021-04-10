using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.BL.Interfaces;

namespace SelfServiceLibrary.API.Services
{
    public class AuthorizationContext : IAuthorizationContext
    {
        public Task<bool> CanBorrow() =>
            Task.FromResult(false);

        public Task<bool> CanBorrowTo() =>
            Task.FromResult(false);

        public Task<bool> CanGrantSelfService() =>
            Task.FromResult(false);

        public Task<bool> CanManageBooks() =>
            Task.FromResult(false);

        public Task<bool> CanManageLibrarians() => 
            Task.FromResult(false);

        public Task<bool> CanReturnFor() =>
            Task.FromResult(false);

        public Task<UserInfoDTO> GetUserInfo()
        {
            throw new System.NotImplementedException();
        }
    }
}
