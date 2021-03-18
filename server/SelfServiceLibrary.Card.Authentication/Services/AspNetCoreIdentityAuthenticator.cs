﻿using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using SelfServiceLibrary.Card.Authentication.Model;
using SelfServiceLibrary.Card.Authentication.Options;
using SelfServiceLibrary.Service.Interfaces;

namespace SelfServiceLibrary.Card.Authentication.Services
{
    public class AspNetCoreIdentityAuthenticator : ICardAuthenticator
    {
        private readonly SignInManager<IdCard> _signInManager;
        private readonly UserManager<IdCard> _userManager;

        public AspNetCoreIdentityAuthenticator(SignInManager<IdCard> signInManager, UserManager<IdCard> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<string?> Authenticate(string cardNumber, string? pin)
        {
            var card = await _userManager.FindByNameAsync(cardNumber);
            var result = await _signInManager.CheckPasswordSignInAsync(card, pin, true);

            return result.Succeeded
                ? card.CvutUsername
                : null;
        }

        public async Task<string?> GetToken(string cardNumber, string? pin)
        {
            var card = await _userManager.FindByNameAsync(cardNumber);
            var result = await _signInManager.CheckPasswordSignInAsync(card, pin, true);

            return result.Succeeded
                ? await _userManager.GenerateUserTokenAsync(card, CardLoginTokenProviderOptions.NAME, "card-auth")
                : null;
        }

        public async Task<string?> AuthenticateWithToken(string cardNumber, string? token)
        {
            var card = await _userManager.FindByNameAsync(cardNumber);
            var isValid = await _userManager.VerifyUserTokenAsync(card, CardLoginTokenProviderOptions.NAME, "card-auth", token);

            return isValid
                ? card.CvutUsername
                : null;
        }
    }
}
