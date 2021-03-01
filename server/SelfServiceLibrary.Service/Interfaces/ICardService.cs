using System.Collections.Generic;
using System.Threading.Tasks;

using SelfServiceLibrary.Service.DTO.Card;

namespace SelfServiceLibrary.Service.Interfaces
{
    public interface ICardService
    {
        Task<bool> Add(string username, AddCardDTO card);
        Task<List<CardListDTO>> GetAll(string username);
        Task<bool> Remove(string username, string cardNumber);
    }
}
