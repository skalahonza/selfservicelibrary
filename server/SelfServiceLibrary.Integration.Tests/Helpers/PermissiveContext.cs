using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.BL.Interfaces;

namespace SelfServiceLibrary.Integration.Tests.Helpers
{
    public class PermissiveContext : IAuthorizationContext
    {
        public Task<bool> CanBorrow()
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanBorrowTo()
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanGrantSelfService()
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanManageContent()
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanManageLibrarians()
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanReturnFor()
        {
            return Task.FromResult(true);
        }

        public Task<UserInfoDTO> GetUserInfo()
        {
            var info = new UserInfoDTO
            {
                Username = "admin",
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@admin.com"
            };
            return Task.FromResult(info);
        }
    }
}
