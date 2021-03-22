using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using SelfServiceLibrary.BL.DTO.Card;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.Card.Authentication.Model;

namespace SelfServiceLibrary.Card.Authentication.Services
{
    public class AspNetCoreIdentityDecorator : ICardService
    {
        private readonly ICardService _decorated;
        private readonly UserManager<IdCard> _userManager;

        public AspNetCoreIdentityDecorator(ICardService decorated, UserManager<IdCard> userManager)
        {
            _decorated = decorated;
            _userManager = userManager;
        }

        public async Task<bool> Add(string username, AddCardDTO card)
        {
            var result = string.IsNullOrEmpty(card.Pin)
                ? await _userManager.CreateAsync(new IdCard(card.Number, username))
                : await _userManager.CreateAsync(new IdCard(card.Number, username), card.Pin);
            return result.Succeeded && await _decorated.Add(username, card);
        }

        public Task<List<CardListDTO>> GetAll(string username) =>
            _decorated.GetAll(username);

        public async Task<bool> Remove(string username, string cardNumber)
        {
            var card = await _userManager.FindByNameAsync(cardNumber);
            var result = await _userManager.DeleteAsync(card);
            return result.Succeeded && await _decorated.Remove(username, cardNumber);
        }
    }
}
