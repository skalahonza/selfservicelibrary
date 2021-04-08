using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.Card.Authentication.Model;
using SelfServiceLibrary.Card.Authentication.Providers;

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
            if (card == null) return null;
            var result = await _signInManager.CheckPasswordSignInAsync(card, pin, true);

            return result.Succeeded
                ? card.CvutUsername
                : null;
        }

        public async Task<string?> GetToken(string cardNumber, string? pin)
        {
            var card = await _userManager.FindByNameAsync(cardNumber);
            if (card == null) return null;

            // card without pin
            if(card.PasswordHash == null && string.IsNullOrEmpty(pin))
            {
                return await _userManager.GenerateUserTokenAsync(card, CardLoginTokenProvider.NAME, "card-auth");
            }

            // card with pin
            var result = await _signInManager.CheckPasswordSignInAsync(card, pin, true);

            return result.Succeeded
                ? await _userManager.GenerateUserTokenAsync(card, CardLoginTokenProvider.NAME, "card-auth")
                : null;
        }

        public async Task<string?> AuthenticateWithToken(string cardNumber, string? token)
        {
            var card = await _userManager.FindByNameAsync(cardNumber);
            if (card == null) return null;
            var isValid = await _userManager.VerifyUserTokenAsync(card, CardLoginTokenProvider.NAME, "card-auth", token);

            return isValid
                ? card.CvutUsername
                : null;
        }
    }
}
