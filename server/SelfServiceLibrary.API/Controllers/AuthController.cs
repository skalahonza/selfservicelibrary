using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using SelfServiceLibrary.API.DTO;
using SelfServiceLibrary.API.Interfaces;

namespace SelfServiceLibrary.API.Controllers
{
    public class AuthController : BaseController
    {
        private readonly ITokenService _service;

        public AuthController(ITokenService service) =>
            _service = service;

        // sign in with code
        [HttpPost]
        public Task<SignInResponse> Login([FromBody] SignIn dto) =>
            _service.GetToken(dto.Code ?? string.Empty);

        [HttpGet]
        public ValueTask<TokenResponse> GetContext() =>
            _service.CheckToken(Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last() ?? string.Empty);
    }
}
