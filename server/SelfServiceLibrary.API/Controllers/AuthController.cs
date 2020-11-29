using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SelfServiceLibrary.API.DTO;
using SelfServiceLibrary.API.Interfaces;

namespace SelfServiceLibrary.API.Controllers
{
    public class AuthController : BaseController
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService) => 
            _tokenService = tokenService;

        [HttpPost("sign-in")]
        [AllowAnonymous]
        public Task<SignInResponse> SignIn([FromBody] SignIn dto) =>
            _tokenService.GetToken(dto.Code ?? string.Empty);

        [HttpPost("refresh")]
        [AllowAnonymous]
        public Task<SignInResponse> Refresh(Refresh dto) =>
            _tokenService.Refresh(dto.RefreshToken ?? string.Empty);
    }
}
