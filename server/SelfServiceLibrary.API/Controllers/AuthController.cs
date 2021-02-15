using System.Threading.Tasks;

using CVUT.Auth;
using CVUT.Auth.Model;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SelfServiceLibrary.API.Controllers
{
    public class AuthController : BaseController
    {
        private readonly ZuulClient _zuul;

        public AuthController(ZuulClient zuul) =>
            _zuul = zuul;

        /// <summary>
        /// oAuth2 sign in using code grant.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("sign-in")]
        [AllowAnonymous]
        public Task<SignInResponse> SignIn([FromBody] SignIn dto) =>
            _zuul.GetToken(dto.Code ?? string.Empty);

        /// <summary>
        /// oAuth2 token refresh
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public Task<SignInResponse> Refresh(Refresh dto) =>
            _zuul.Refresh(dto.RefreshToken ?? string.Empty);
    }
}
