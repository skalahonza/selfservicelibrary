using System.Threading.Tasks;

namespace SelfServiceLibrary.Service.Interfaces
{

    public interface ICardAuthenticator
    {
        Task<string?> Authenticate(string cardNumber, string? pin);
        Task<string?> AuthenticateWithToken(string cardNumber, string? token);
        Task<string?> GetToken(string cardNumber, string? pin);
    }
}
