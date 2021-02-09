using SelfServiceLibrary.BL.Model;

using System.Threading.Tasks;

namespace SelfServiceLibrary.BL.Interfaces
{
    public interface IUserContextService
    {
        Task<UserContext> GetInfo(string username);
    }
}
