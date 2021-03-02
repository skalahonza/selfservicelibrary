using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using SelfServiceLibrary.Card.Authentication.Model;
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
            var result = await _signInManager.CheckPasswordSignInAsync(card, pin, false);

            // TODO handle failure
            return result.Succeeded
                ? card.CvutUsername
                : null;
        }
    }
}
