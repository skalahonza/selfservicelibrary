﻿using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Mvc;

using SelfServiceLibrary.BL.Interfaces;

namespace SelfServiceLibrary.API.Controllers
{
    public record CardLoginRequest(string CardNumber, string? Pin);
    public record CardLoginResponse(string RedirectUrl);

    public class AuthController : BaseController
    {
        private readonly ICardAuthenticator _authenticator;

        public AuthController(ICardAuthenticator authenticator) =>
            _authenticator = authenticator;

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<CardLoginResponse>> Login([FromBody] CardLoginRequest data)
        {
            var token = await _authenticator.GetToken(data.CardNumber, data.Pin);
            return (token == null)
                ? Unauthorized()
                : Ok(new CardLoginResponse($"/login?card={data.CardNumber}&token={HttpUtility.UrlEncode(token)}"));
        }
    }
}
