using SelfServiceLibrary.API.DTO;

using System.Threading.Tasks;

namespace SelfServiceLibrary.API.Interfaces
{
    public interface ITokenService
    {
        Task<SignInResponse> GetToken(string code);
        ValueTask<TokenResponse> CheckToken(string token);
    }
}
