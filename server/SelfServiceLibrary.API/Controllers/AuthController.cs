using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
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

        [HttpPost]
        [AllowAnonymous]
        public Task<SignInResponse> Login([FromBody] SignIn dto) =>
            _service.GetToken(dto.Code ?? string.Empty);

        [HttpGet]
        public (IEnumerable<string> Roles, string Name) GetContext()
        {
            return (User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value), User.Identity.Name);
        }
    }
}
