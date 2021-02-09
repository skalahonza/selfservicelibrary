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

        /// <summary>
        /// oAuth2 sign in using code grant.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("sign-in")]
        [AllowAnonymous]
        public Task<SignInResponse> SignIn([FromBody] SignIn dto) =>
            _tokenService.GetToken(dto.Code ?? string.Empty);

        /// <summary>
        /// oAuth2 token refresh
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public Task<SignInResponse> Refresh(Refresh dto) =>
            _tokenService.Refresh(dto.RefreshToken ?? string.Empty);
    }
}
