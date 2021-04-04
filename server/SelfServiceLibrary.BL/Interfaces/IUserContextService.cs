using System.Collections.Generic;
using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.User;

namespace SelfServiceLibrary.BL.Interfaces
{
    public interface IUserContextService
    {
        /// <summary>
        /// Suggest list of users based on searched term
        /// </summary>
        /// <param name="term">Term to search users with</param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<List<UserInfoDTO>> Suggest(string term, int limit = 10);
    }
}
