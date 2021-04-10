using System.Collections.Generic;
using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.Guest;
using SelfServiceLibrary.BL.DTO.User;

namespace SelfServiceLibrary.BL.Interfaces
{
    public interface IGuestService
    {
        Task Add(GuestDTO guest);
        Task Delete(string id);
        Task<List<GuestDTO>> GetAll();
        Task<List<UserInfoDTO>> Suggest(string term, int limit = 10);
        Task Update(GuestDTO guest);
    }
}