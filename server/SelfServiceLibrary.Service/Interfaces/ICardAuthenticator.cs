using System.Threading.Tasks;

namespace SelfServiceLibrary.Service.Interfaces
{

    public interface ICardAuthenticator
    {
        Task<string?> Authenticate(string cardNumber, string? pin);
    }
}
