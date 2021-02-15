using System.Threading.Tasks;

using SelfServiceLibrary.Domain.Entities;

namespace SelfServiceLibrary.Service.Interfaces
{
    public interface IUserProvider
    {
        Task<User> Get(string username);
    }
}
