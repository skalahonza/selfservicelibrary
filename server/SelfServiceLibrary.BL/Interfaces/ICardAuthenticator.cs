using System.Threading.Tasks;

namespace SelfServiceLibrary.BL.Interfaces
{
    public interface ICardAuthenticator
    {
        Task<string?> Authenticate(string cardNumber, string? pin);
        Task<string?> AuthenticateWithToken(string cardNumber, string? token);
        Task<string?> GetToken(string cardNumber, string? pin);
        Task<bool> Exists(string cardNumber);
        Task<bool> HasPin(string cardNumber);
    }
}
